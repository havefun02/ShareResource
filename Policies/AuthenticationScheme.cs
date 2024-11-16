using ShareResource.Models.Entities;
using ShareResource.Interfaces;
using ShareResource.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Encodings.Web;
namespace ShareResource
{
    public class AuthenticationScheme : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IJwtService<User> _jwtService;
        private readonly ITokenService<Token> _tokenService;
        private readonly IEncryptionService _encryptionService;
        private string? refreshToken;
        private string? accessToken;

        public AuthenticationScheme(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IJwtService<User> jwtService,
            ITokenService<Token> tokenService, IEncryptionService encryptionService
            )
            : base(options, logger, encoder)
        {
            _encryptionService = encryptionService;
            _jwtService = jwtService;
            _tokenService = tokenService;
            refreshToken = "";
            accessToken = "";
        }
        private async Task<AuthenticateResult> HandleExpiredTokenAsync()
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return AuthenticateResult.Fail("Cannot find your refresh token");
            }

            try
            {
                var tokenInfo = await _tokenService.GetTokenInfo(refreshToken);
                if (tokenInfo == null || tokenInfo.ExpiredAt < DateTime.UtcNow ||tokenInfo.IsRevoked)
                {
                    return AuthenticateResult.Fail("Invalid refresh token");
                }

                var newAccessToken = _jwtService.GenerateAccessToken(tokenInfo.User!);
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    var newRefreshToken = await _tokenService.UpdateTokenAsync(tokenInfo);
                    Context.Response.Cookies.Append("accessToken", _encryptionService.EncryptData(newAccessToken), new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });

                    Context.Response.Cookies.Append("refreshToken", _encryptionService.EncryptData(newRefreshToken.RefreshToken!), new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    var principal = _jwtService.ValidateAccessToken(newAccessToken);
                    var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.Fail("Failed to generate new access token");
            }
            catch (ArgumentException ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Error occured while trying to validate reset token: {ex.Message}");
            }
        }
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Context.Response.Redirect("/api/v1/auths/login");
            return Task.CompletedTask;
        }
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            Context.Response.Redirect("/");
            return Task.CompletedTask;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var isLoginEndpoint = Context.Request.Path.StartsWithSegments("/api/v1/auths/login");
            accessToken = Context.Request.Cookies["accessToken"];
            refreshToken = Context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return AuthenticateResult.Fail("No token provided");
            }
            try
            {
                accessToken = _encryptionService.DecryptData(accessToken);
                refreshToken = _encryptionService.DecryptData(refreshToken);
                var tokenInfo = await _tokenService.GetTokenInfo(refreshToken);
                if (tokenInfo == null || tokenInfo.IsRevoked || tokenInfo.ExpiredAt< DateTime.UtcNow)
                {
                    return AuthenticateResult.Fail("No token provided");
                }
                var principal = _jwtService.ValidateAccessToken(accessToken);
                var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");

                if (isLoginEndpoint)
                {
                    Context.Response.Redirect("/");
                }
                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                var verifyExpired = await HandleExpiredTokenAsync();
                if (verifyExpired.Succeeded)
                {
                    if (isLoginEndpoint)
                    {
                        Context.Response.Redirect("/");
                    }
                    return verifyExpired;
                }
                return verifyExpired;

            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}
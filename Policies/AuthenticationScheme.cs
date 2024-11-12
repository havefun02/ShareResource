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
        }

        bool NeedAuthorize(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null) return false;

            var controllerDescriptor = endpoint.Metadata
                .GetMetadata<ControllerActionDescriptor>()
                ?.ControllerTypeInfo;

            var hasAuthorizeOnController = controllerDescriptor?
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any() ?? false;

            var actionDescriptor = endpoint.Metadata
                .GetMetadata<ControllerActionDescriptor>()?.MethodInfo;

            var hasAuthorizeOnAction = actionDescriptor?
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any() ?? false;

            return hasAuthorizeOnController || hasAuthorizeOnAction;
        }

        private async Task<AuthenticateResult> HandleExpiredTokenAsync()
        {
            var refreshToken = Context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return AuthenticateResult.Fail("Cannot find your refresh token");
            }

            try
            {
                refreshToken = _encryptionService.DecryptData(refreshToken);
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
                    Context.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = true,
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
            return Task.CompletedTask;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var isLoginEndpoint = Context.Request.Path.StartsWithSegments("/auth/login");
            if (NeedAuthorize(Context) || isLoginEndpoint)
            {
                var token = Context.Request.Cookies["accessToken"];
                var refresh = Context.Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refresh))
                {
                    Context.Response.Cookies.Delete("isLogged");
                    return AuthenticateResult.Fail("No token provided");
                }
                try
                {
                    token = _encryptionService.DecryptData(token);
                    refresh=_encryptionService.DecryptData(refresh);
                    var tokenInfo = await _tokenService.GetTokenInfo(refresh);
                    if (tokenInfo == null || tokenInfo.IsRevoked || tokenInfo.ExpiredAt< DateTime.UtcNow)
                    {
                        Context.Response.Cookies.Delete("isLogged");
                        return AuthenticateResult.Fail("No token provided");
                    }
                    var principal = _jwtService.ValidateAccessToken(token);
                    var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");

                    Context.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    if (isLoginEndpoint)
                    {
                        Context.Response.Redirect("/User/Profile");
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
                            Context.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                            {
                                HttpOnly = true,
                                //Secure = true,
                                SameSite = SameSiteMode.Strict,
                            });

                            Context.Response.Redirect("/User/Profile");
                        }
                        return verifyExpired;
                    }
                    Context.Response.Cookies.Delete("isLogged");
                    return verifyExpired;

                }
                catch (Exception ex)
                {
                    Context.Response.Cookies.Delete("isLogged");
                    return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
                }
            }
            else
            {
                return AuthenticateResult.NoResult();
            }
        }
    }
}
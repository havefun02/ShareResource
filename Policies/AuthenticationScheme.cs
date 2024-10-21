using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShareResource.Decorators;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ShareResource.Policies
{
    public class AppAuthenticationHandler:AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IJwtService<User> _jwtService;
        private readonly IAuthService<User,Token>  _authService;

        public AppAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IJwtService<User> jwtService,
            IAuthService<User, Token> authService
            )
            : base(options, logger, encoder)
        {
            _jwtService = jwtService;
            _authService = authService;
        }


        bool CanBeIgnored(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null) return false;

            var controllerActionDescriptor = endpoint.Metadata
                .GetMetadata<ControllerActionDescriptor>();

            var result= controllerActionDescriptor?.MethodInfo
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any() ?? false;
            return result;
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
                var tokenInfo = await _authService.GetTokenInfoAsync(refreshToken);
                if (tokenInfo == null ||tokenInfo.ExpiredAt < DateTime.UtcNow)
                {
                    return AuthenticateResult.Fail("Invalid refresh token");
                }

                var newAccessToken = _jwtService.GenerateToken(tokenInfo.User!);
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    Context.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });

                    var newRefreshToken = await _authService.UpdateTokenAsync(tokenInfo);
                    Context.Response.Cookies.Append("refreshToken", newRefreshToken.RefreshToken!, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    var principal = _jwtService.ValidateToken(newAccessToken);
                    var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.Fail("Failed to generate new access token");
            }
            catch (ArgumentException ex)
            {
                return AuthenticateResult.Fail("User not found with error " +ex.Message);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Error refreshing token: {ex.Message}");
            }
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (CanBeIgnored(Context))
            {
                var token = Context.Request.Cookies["accessToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("No token provided");
                }
                try
                {
                    var principal = _jwtService.ValidateToken(token);
                    if (principal != null)
                    {
                        var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid token");
                    }
                }
                catch (SecurityTokenExpiredException)
                {
                    return await HandleExpiredTokenAsync();
                        
                }
                catch(ArgumentException)
                {
                    return AuthenticateResult.Fail("User not found");
                }
                catch (Exception ex)
                {
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

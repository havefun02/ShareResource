using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using System.Security.Claims;


namespace ShareResource.Controllers
{
    [Route("api/v1/auths")]
    public class AuthController:Controller
    {

        private readonly IAuthService<User> _authService;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;


        public AuthController(IAuthService<User> authService, IMapper mapper,IEncryptionService encryptionService)
        {
            _mapper = mapper;
            _authService = authService;
            _encryptionService= encryptionService;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        [HttpPost("login")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var sessionId = HttpContext.Request.Cookies["ssid"];
                if (string.IsNullOrEmpty(sessionId))
                {
                    View("Login", loginDto);
                }

                var (access, refresh) = await _authService.Login(loginDto);
                if (access== null ||refresh==null) return View("Login",loginDto);
                HttpContext.Response.Cookies.Append("accessToken", _encryptionService.EncryptData(access), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                HttpContext.Response.Cookies.Append("refreshToken", _encryptionService.EncryptData(refresh), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                HttpContext.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                var returnUrl = Request.Query["returnUrl"].ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect("/");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            catch (Exception)
            {
                return View("Login",loginDto);
            }
        }
        [HttpPost("register")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register( RegisterDto registerDto)
        {
            Console.WriteLine("register");
            if (!ModelState.IsValid)
            {
                return View("Register", registerDto);
            }
            try
            {
                var registerResult = await _authService.Register(registerDto);
                if (registerResult)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    return View("Register", registerDto);
                }
            }
            catch (Exception)
            {
                return View("Register", registerDto);
            }
        }
        [Authorize]
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User ID not found in claims.");
                }
                var updatePasswordResult = await _authService.UpdatePassword(updatePasswordDto, userId);
                if (updatePasswordResult) return Ok();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("change-password")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ChangePassword( ChangePasswordDto changePasswordDto)
        {
            try
            {
                var changePasswordResult = await _authService.ChangePassword(changePasswordDto);
                if (changePasswordResult) return RedirectToAction("Login");
                else return View("ChangePassword",changePasswordDto);
            }
            catch (Exception)
            {
                return View("ChangePassword", changePasswordDto);
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User ID not found in claims.");
                }
                var logoutResult = await _authService.Logout(userId);
                if (logoutResult)
                {
                    HttpContext.Response.Cookies.Delete("accessToken");
                    HttpContext.Response.Cookies.Delete("refreshToken");
                    HttpContext.Response.Cookies.Delete("isLogged");

                    return RedirectToAction("Login");
                }
                else return NoContent();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

    }
}

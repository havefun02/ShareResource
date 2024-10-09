using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using System.Security.Claims;


namespace ShareResource.Controllers
{
    [ApiController]
    [Route("api/v1/auths")]
    public class AuthController:ControllerBase
    {

        private readonly IAuthService<User,Token> _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService<User, Token> authService,IMapper mapper) {
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var (access, refresh) = await _authService.Login(loginDto);
                HttpContext.Response.Cookies.Append("accessToken", access, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                HttpContext.Response.Cookies.Append("refreshToken", refresh, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                return Ok();
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserResultDto>> Register([FromBody] RegisterDto registerDto)
        {

            try
            {
                var registerResult=await this._authService.Register(registerDto);
                var returnResult=_mapper.Map<UserResultDto>(registerResult);
                return Ok(returnResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                var updatePasswordResult =await this._authService.UpdatePassword(updatePasswordDto, userId);
                if (updatePasswordResult) return Ok();
                else return BadRequest();
            }
            catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var changePasswordResult=await _authService.ChangePassword(changePasswordDto);
                if (changePasswordResult) return Ok();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                var logoutResult = await this._authService.Logout(userId);
                if (logoutResult) {
                    HttpContext.Response.Cookies.Delete("accessToken");
                    HttpContext.Response.Cookies.Delete("refreshToken");
                    return Ok();
                }
                else return BadRequest();
            }
            catch(Exception ex) { 
                return BadRequest(ex.Message); 
            }
        }

    }
}

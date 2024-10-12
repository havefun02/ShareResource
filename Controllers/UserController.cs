using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Services;
using System.Security.Claims;

namespace ShareResource.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService<User> _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService<User> userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }


        [Authorize]
        [HttpGet("basic-users")]
        public async Task<ActionResult<UserResultDto>> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var user = await _userService.GetUserProfile(userId);
                var userResult = _mapper.Map<UserResultDto>(user);
                return Ok(userResult);
            }
            catch (Exception ex) {
                return NotFound($"User with ID {userId} not found with {ex.Message}.");
            }
        }
        [Authorize]
        [HttpPut("basic-users")]
        public async Task<ActionResult<UserResultDto>> EditProfile([FromBody] UserDto userDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (userDto == null || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User data cannot be null.");
            }
            try
            {
                var updatedUser = await _userService.EditProfile(userDto, userId);
                var userResult = _mapper.Map<UserResultDto>(updatedUser);
                return Ok(userResult);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("basic-users")]
        public async Task<IActionResult> DeleteProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                await _userService.DeleteProfile(userId);
                HttpContext.Response.Cookies.Delete("accessToken");
                HttpContext.Response.Cookies.Delete("refreshToken");
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

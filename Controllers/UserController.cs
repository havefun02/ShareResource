using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;

namespace ShareResource.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController:ControllerBase
    {
        private readonly IUserService<User> _userService;
        private readonly IAdminService<User> _adminService;

        public UserController(IUserService<User> userService, IAdminService<User> adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userService.GetUserProfile(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return Ok(user);
        }

        [HttpPut("")]
        public async Task<IActionResult> EditProfile([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data cannot be null.");
            }

            var updatedUser = await _userService.EditProfile(userDto);
            return Ok(updatedUser);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            await _userService.DeleteProfile(userId);
            return NoContent();
        }

        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto updateUserRoleDto)
        {
            if (updateUserRoleDto == null)
            {
                return BadRequest("Role update data cannot be null.");
            }

            await _adminService.UpdateUserRole(updateUserRoleDto);
            return Ok("User role updated successfully.");
        }

        [HttpDelete("delete-account/{userId}")]
        public async Task<IActionResult> DeleteUserAccount(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            await _adminService.DeleteUserAccount(userId);
            return NoContent();
        }
    }
}

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
    [Route("/api/v1/admins")]
    public class AdminController:ControllerBase
    {
        private readonly IAdminService<User> _adminService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService<User> adminService,IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }
        [Authorize(Policy = "OwnerOnly")]
        [HttpPut("/user-managements/{editedUserId}/update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto updateUserRoleDto, string editedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            if (userId == null || updateUserRoleDto.RoleId == null || editedUserId == null)
            {
                return BadRequest();
            }

            await _adminService.UpdateUserRole(updateUserRoleDto, userId, editedUserId);
            return Ok("User role updated successfully.");
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpDelete("/user-managements/{editedUserId}")]
        public async Task<IActionResult> DeleteUserAccount(string editedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(editedUserId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            await _adminService.DeleteUserAccount(userId, editedUserId);
            return NoContent();
        }

    }
}

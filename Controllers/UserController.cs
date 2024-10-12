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
        private readonly IAdminService<User> _adminService;
        private readonly IRoleService<Role> _roleService;
        private readonly IMapper _mapper;



        public UserController(IUserService<User> userService, IAdminService<User> adminService, IRoleService<Role> roleService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
            _adminService = adminService;
            _roleService = roleService;
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

        [Authorize(Policy = "OwnerOnly")]
        [HttpPut("admins/user-managements/{editedUserId}/update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto updateUserRoleDto, string editedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            
            if (userId == null || updateUserRoleDto.RoleId==null || editedUserId==null) 
            {
                return BadRequest();
            }

            await _adminService.UpdateUserRole(updateUserRoleDto,userId,editedUserId);
            return Ok("User role updated successfully.");
        }

        [Authorize(Policy ="OwnerOnly")]
        [HttpDelete("admins/user-managements/{editedUserId}")]
        public async Task<IActionResult> DeleteUserAccount(string editedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(editedUserId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            await _adminService.DeleteUserAccount(userId,editedUserId);
            return NoContent();
        }
        [HttpDelete("admins/role-managements/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var deleteStatus = await _roleService.DeleteRole(roleId, userId);
                if (deleteStatus != 0) return Ok("Delete role successfully");
                else return BadRequest("Cant not delete the role");
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("admins/role-managements")]
        public async Task<ActionResult<List<RoleResultDto>>> GetRoles()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var roleResult = await _roleService.GetRoles(userId);
                var returnResult = _mapper.Map<List<RoleResultDto>>(roleResult);
                return Ok(returnResult);
            }catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("admins/role-managements/{roleId}")]
        public async Task<ActionResult<RoleResultDto>> UpdateRole(string roleId,[FromBody] UpdateRoleDto updateRoleDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return BadRequest("Arguements cannot be null or empty.");
            }
            try
            {
                var updateStatus = await _roleService.UpdateRole(updateRoleDto,roleId, userId);
                if (updateStatus != null) {

                    var roleResult=_mapper.Map<RoleResultDto>(updateStatus);
                    return Ok(roleResult);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("admins/role-managements/")]
        public async Task<ActionResult<RoleResultDto>> CreateRole([FromBody] RoleDto roleDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            try
            {
                var createStatus=await _roleService.CreateRole(roleDto, userId);
                if (createStatus != null) { 
                    var roleResult=_mapper.Map<RoleResultDto>(createStatus);
                    return Ok(roleResult);
                }
                else { return BadRequest(); }
            }
            catch(Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
    }
}

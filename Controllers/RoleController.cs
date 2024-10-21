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
    [Route("api/v1/admins")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService<Role> _roleService;
        private readonly IMapper _mapper;
        public RoleController(IRoleService<Role> roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpDelete("/role-managements/{roleId}")]
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("/role-managements")]
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
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("/role-managements/{roleId}")]
        public async Task<ActionResult<RoleResultDto>> UpdateRole(string roleId, [FromBody] UpdateRoleDto updateRoleDto)
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
                var updateStatus = await _roleService.UpdateRole(updateRoleDto, roleId, userId);
                if (updateStatus != null)
                {

                    var roleResult = _mapper.Map<RoleResultDto>(updateStatus);
                    return Ok(roleResult);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("/role-managements/")]
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
                var createStatus = await _roleService.CreateRole(roleDto, userId);
                if (createStatus != null)
                {
                    var roleResult = _mapper.Map<RoleResultDto>(createStatus);
                    return Ok(roleResult);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

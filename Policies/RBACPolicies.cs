using Microsoft.AspNetCore.Authorization;
using ShareResource.Services;
using System.Security.Claims;

namespace ShareResource.Policies
{
    public class RBACPolicies
    {

        public abstract class RoleRequirementBase : IAuthorizationRequirement
        {
            protected readonly RolePermissionsCacheService _service;

            public RoleRequirementBase(RolePermissionsCacheService service)
            {
                _service = service;
            }
            public abstract Task<bool> IsAuthorized(string roleId);
        }

        public class ExecutePermissionOnly : RoleRequirementBase
        {
            public ExecutePermissionOnly(RolePermissionsCacheService service) : base(service)
            {
            }
            public override async Task<bool> IsAuthorized(string roleId)
            {
                var rps = await this._service.GetPermissionsForRoleAsync(roleId);
                if (rps != null && rps.Contains("Execute"))
                {
                    return true;
                }
                return false;
            }
        }

        public class OwnerOnlyRequirement : RoleRequirementBase
        {
            public OwnerOnlyRequirement(RolePermissionsCacheService service) : base(service)
            {
            }
            public override async Task<bool> IsAuthorized(string roleId)
            {
                var roles = await this._service.GetRolesAsync();
                if (roles != null && roles.Contains(roleId) && roleId == "Owner")
                {
                    return true;
                }
                return false;
            }
        }
        public class AdminOnlyRequirement : RoleRequirementBase
        {
            public AdminOnlyRequirement(RolePermissionsCacheService service) : base(service)
            {
            }
            public override async Task<bool> IsAuthorized(string roleId)
            {

                var roles = await this._service.GetRolesAsync();
                if (roles != null && roles.Contains(roleId) && roleId == "Admin")
                {
                    return true;
                }
                return false;
            }
        }
        public class RoleAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
            where TRequirement : RoleRequirementBase
        {
            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
            {
                var roleClaim = context.User.FindFirst(ClaimTypes.Role);
                var userRole = roleClaim?.Value;

                if (string.IsNullOrEmpty(userRole))
                {
                    context.Fail();
                    return;
                }

                var result = await requirement.IsAuthorized(userRole);

                if (result)
                {
                    context.Succeed(requirement);
                    return;
                }
                context.Fail();
            }
    }
}
}

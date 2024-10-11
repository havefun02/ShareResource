using AutoMapper;
using CRUDFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using System.Linq;

namespace ShareResource.Services
{
    public class RoleService : IRoleService<Role>
    {
        private readonly IRepository<Role,AppDbContext> _roleRepository;
        private readonly IRepository<RolePermission, AppDbContext> _rolePermissionsRepository;

        private readonly IMapper _mapper;

        public RoleService(IRepository<Role, AppDbContext> roleRepository,IMapper mapper, IRepository<RolePermission, AppDbContext> rolePermissionsRepository) {
            
            _roleRepository = roleRepository;
            _rolePermissionsRepository = rolePermissionsRepository;
            _mapper = mapper;
        }
        public async Task<Role> CreateRole(RoleDto roleDto, string adminId)
        {
            if (roleDto == null || roleDto.RoleName == string.Empty || roleDto.RolePermissionsId==null)
            {
                throw new ArgumentException("Invalid role data.");
            }
            var roleId=Guid.NewGuid().ToString();   
            var rolePermissions = new List<RolePermission>();
            foreach (var p in roleDto.RolePermissionsId)
            {
                var rp=new RolePermission();
                rp.RoleId= roleId;
                rp.PermissionId=p;
                rolePermissions.Add(rp);
            }
            var newRole = new Role
            {
                RoleId = roleId,
                RoleName = roleDto.RoleName,
                RolePermissions = rolePermissions // Assuming RoleDto contains a list of permissions
            };

            var createdRole = await _roleRepository.CreateAsync(newRole);
            return createdRole;
        }

        public async Task<int> DeleteRole(string roleId, string adminId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("Role ID cannot be null or empty.");
            }

            var roleToDelete = await _roleRepository.FindOneById(roleId);
            if (roleToDelete == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            var status=await _roleRepository.Delete(roleToDelete.RoleId!);
            if (status==0) { throw new InvalidOperationException("Cant delete the role"); }
            return status;
        }

        public async Task<List<Role>> GetRoles(string adminId)
        {
            var roleDbContext = _roleRepository.GetDbSet();
            var roles = await roleDbContext.Include(r=>r.RolePermissions!).ToListAsync();
            return roles;
        }

        public async Task<Role> UpdateRole(UpdateRoleDto updateRole,string roleId, string adminId)
        {
            if (updateRole == null || string.IsNullOrWhiteSpace(updateRole.RoleName)|| string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("Invalid role update data.");
            }
            var roleToUpdate = await _roleRepository.FindOneById(roleId);
            if (roleToUpdate == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            roleToUpdate.RoleName = updateRole.RoleName;
            roleToUpdate.RoleDescription= updateRole.RoleDescription;
            var rpContext=_rolePermissionsRepository.GetDbSet();
            var existingPermissions=await rpContext.Where(rp=>rp.RoleId==roleId).Select(rp=>rp.PermissionId).ToListAsync();

            var rolePermissions = new List<RolePermission>();
            foreach (var p in updateRole.RolePermissionsId!)
            {
                if (!existingPermissions.Contains(p)) 
                {
                    var rp = new RolePermission();
                    rp.RoleId = roleId;
                    rp.PermissionId = p;
                    rolePermissions.Add(rp);
                }
            }
            roleToUpdate.RolePermissions = rolePermissions;
            var permissionsToRemove = existingPermissions
            .Where(rp =>!updateRole.RolePermissionsId.Contains(rp!))
            .ToList();
            foreach (var permission in permissionsToRemove)
            {
                var deleteResult = await rpContext.Where(rp => rp.PermissionId == permission && rp.RoleId == roleId).ExecuteDeleteAsync();
            }
            var newRole = await _roleRepository.Update(roleToUpdate);
            var roleDbContext = _roleRepository.GetDbSet();
            var roles = await roleDbContext.Include(r => r.RolePermissions!).SingleOrDefaultAsync(rp => rp.RoleId == roleId);
            if (roles == null)
            {
                throw new InvalidOperationException("Error occured while updating role");
            }
            return roles;
        }
    }
}

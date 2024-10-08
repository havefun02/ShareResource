using AutoMapper;
using CRUDFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;

namespace ShareResource.Services
{
    public class RoleService : IRoleService<Role>
    {
        private readonly IRepository<Role,AppDbContext> _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRepository<Role, AppDbContext> roleRepository,IMapper mapper) {
            
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        public async Task<Role> CreateRole(RoleDto roleDto, string adminId)
        {
            if (roleDto == null || roleDto.RoleId == string.Empty || roleDto.RolePermissions==null)
            {
                throw new ArgumentException("Invalid role data.");
            }
            var rolePermissions = new List<RolePermission>();
            foreach (var p in roleDto.RolePermissions)
            {
                var rp=new RolePermission();
                rp.RoleId=roleDto.RoleId;
                rp.PermissionId=p.PermissionId;
                rolePermissions.Add(rp);
            }
            var newRole = new Role
            {
                RoleId = roleDto.RoleId,
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

            var status=await _roleRepository.Delete(roleToDelete);
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
            var rolePermissions = new List<RolePermission>();
            foreach (var p in updateRole.RolePermissions!)
            {
                var rp = new RolePermission();
                rp.RoleId = roleId;
                rp.PermissionId = p.PermissionId;
                rolePermissions.Add(rp);
            }
            roleToUpdate.RolePermissions=rolePermissions;
            var newRole=await _roleRepository.Update(roleToUpdate);
            return newRole;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShareResource.Database;
using System.Data;

namespace ShareResource.Services
{
    public class RolePermissionsCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;
        private const string RolePermissionsKey = "RolePermissions";
        private const string RolesKey = "Roles";

        public RolePermissionsCacheService(IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider != null ? serviceProvider : throw new NullReferenceException();
            _memoryCache = memoryCache != null ? memoryCache : throw new NullReferenceException();
        }
        public async Task<List<string>> GetPermissionsForRoleAsync(string role)
        {
            if (!_memoryCache.TryGetValue(RolePermissionsKey, out Dictionary<string, List<string>>? rolePermissionsCache))
            {
                rolePermissionsCache = await LoadRolePermissionsFromDbAsync();

                _memoryCache.Set(RolePermissionsKey, rolePermissionsCache);
            }

            return rolePermissionsCache!.TryGetValue(role, out var permissions) ? permissions : new List<string>();
        }
        public async Task<List<string>> GetRolesAsync()
        {
            if (!_memoryCache.TryGetValue(RolesKey, out List<string>? roles))
            {
                roles = await LoadRolesListAsync();
                _memoryCache.Set(RolesKey, roles);

            }
            return roles != null ? roles : new List<string>();

        }

        private async Task<List<string>> LoadRolesListAsync()
        {
            using (var scopeService = _serviceProvider.CreateScope())
            {
                var dbContext = scopeService.ServiceProvider.GetRequiredService<AppDbContext>();
                return await dbContext.Roles.Select(r => r.RoleId).ToListAsync();
            }
        }

        private async Task<Dictionary<string, List<string>>> LoadRolePermissionsFromDbAsync()
        {
            using (var scopeService = _serviceProvider.CreateScope())
            {
                var dbContext = scopeService.ServiceProvider.GetRequiredService<AppDbContext>();
                return await dbContext.Roles.Include(r => r.RolePermissions).ToDictionaryAsync(r => r.RoleName, r => r.RolePermissions!.Select(rp => rp.PermissionId).ToList());
            }
        }

        public void ClearCache()
        {
            _memoryCache.Remove(RolesKey);
            _memoryCache.Remove(RolePermissionsKey);
        }

    }
}
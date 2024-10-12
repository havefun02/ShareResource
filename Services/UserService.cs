using AutoMapper;
using CRUDFramework.Interfaces;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Exceptions ;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ShareResource.Services
{
    public class UserService : IUserService<User>, IAdminService<User>
    {
        private readonly IRepository<User, AppDbContext> _userRepository;
        private readonly IRepository<Role, AppDbContext> _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IRepository<User, AppDbContext> userRepository,
            IRepository<Role, AppDbContext> roleRepository,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Edits the user profile based on the provided UserDto.
        /// </summary>
        /// <param name="userDto">The DTO containing user information to update.</param>
        /// <returns>The updated User entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when userDto is null.</exception>
        /// <exception cref="ArgumentException">Thrown when user not found during update.</exception>
        public async Task<User> EditProfile(UserDto userDto,string userId)
        {
            try
            {
                if (userDto == null || string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("Invalid data requirement");
                var userData = await this._userRepository.FindOneById(userId);
                if (userData == null)
                {
                    throw new InvalidOperationException("User does not exist");
                }
                userData.UserName = userDto.UserName;
                userData.UserPhone = userDto.UserPhone;
                var user = await _userRepository.Update(userData);
                var userContext = this._userRepository.GetDbSet();

                user = await userContext.Include(u => u.UserRole!).ThenInclude(r => r.RolePermissions!).SingleOrDefaultAsync(u => u.UserId == userId);
                if (user == null) { throw new InvalidOperationException(); }
                return user;
            }
            catch(Exception ex) { throw new InternalException("Edit profile failed. ", ex); }
        }

        /// <summary>
        /// Deletes a user profile by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when userId is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when user not found or could not be deleted.</exception>
        public async Task DeleteProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            try
            {
                var userContext = this._userRepository.GetDbSet();
                var user = await userContext.Include(u => u.UserRole!).ThenInclude(r => r.RolePermissions!).SingleOrDefaultAsync(u => u.UserId == userId);
                if (user != null && user.UserRoleId == "Admin")
                {
                    throw new InvalidOperationException("Cannot delete admin account");
                }
                var status = await _userRepository.Delete(userId);
                if (status == 0)
                {
                    throw new ArgumentException("User not found or could not be deleted.");
                }
            }
            catch(Exception ex) {
                throw new InternalException("Fail to delete ", ex);
            }

        }

        /// <summary>
        /// Retrieves a user profile by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The User entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when userId is null or empty.</exception>
        public async Task<User> GetUserProfile(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
                var userContext = this._userRepository.GetDbSet();

                var user = await userContext.Include(u => u.UserRole!).ThenInclude(r => r.RolePermissions!).SingleOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }
                return user;
            }
            catch (Exception ex) {
                throw new InternalException("Cant find user ", ex);
            }
        }

        /// <summary>
        /// Updates the user role based on the provided UpdateUserRoleDto.
        /// </summary>
        /// <param name="updateUserRoleDto">The DTO containing user ID and role ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when updateUserRoleDto is null.</exception>
        /// <exception cref="ArgumentException">Thrown when user does not exist.</exception>
        public async Task UpdateUserRole(UpdateUserRoleDto updateUserRoleDto,string adminId,string userId)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();

                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(adminId)) throw new ArgumentNullException();

                var admin = await userContext.Include(u => u.UserRole).SingleOrDefaultAsync(u => u.UserId == adminId);
                if (admin == null) throw new ArgumentException("Admin does not exist");
                if (admin.UserRole == null || admin.UserRole.RoleName != "Owner")
                {
                    throw new UnauthorizedAccessException("Only Owner can update user roles.");
                }


                var user = await userContext.Include(u => u.UserRole).SingleOrDefaultAsync(u => u.UserId == userId && u.UserRole!.RoleName != "Owner");
                if (user == null) throw new ArgumentException("Cannot update user due to mismatch permission or user does not exist");

                user.UserRoleId = updateUserRoleDto.RoleId;
                await _userRepository.Update(user);
            }
            catch(Exception ex)
            {
                throw new InternalException("Cant update account ", ex);
            }
        }

        /// <summary>
        /// Deletes a user account by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user account to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when userId is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when user does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when user could not be deleted.</exception>
        public async Task DeleteUserAccount(string adminId, string userId)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();

                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(adminId)) throw new ArgumentNullException();

                // Get the admin and check role
                var admin = await userContext.Include(u => u.UserRole).SingleOrDefaultAsync(u => u.UserId == adminId);
                if (admin == null) throw new ArgumentException("Admin does not exist");

                if (admin.UserRole == null || admin.UserRole.RoleName != "Owner")
                {
                    throw new UnauthorizedAccessException("Only Owner can delete user accounts.");
                }

                // Find the user to delete (must be Guest role to delete)
                var user = await userContext.Include(u => u.UserRole).SingleOrDefaultAsync(u => u.UserId == userId && u.UserRole!.RoleName != "Owner");
                if (user == null) throw new ArgumentException("Cannot delete user due to mismatch permission or user does not exist");

                // Delete user account
                var status = await _userRepository.Delete(user.UserId!);
                if (status == 0)
                {
                    throw new InvalidOperationException("User could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                throw new InternalException("Cannot delete account", ex);
            }
        }

    }
}

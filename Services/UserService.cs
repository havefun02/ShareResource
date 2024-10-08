using AutoMapper;
using CRUDFramework.Interfaces;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
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
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var userData = await this._userRepository.FindOneById(userId);
            if (userData == null) {
                throw new InvalidOperationException("User does not exist");
            }
            userData.UserName=userDto.UserName;
            userData.UserPhone = userDto.UserPhone;
            var user = await _userRepository.Update(userData);
            return user;
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

            var status = await _userRepository.Delete(userId);
            if (status == 0)
            {
                throw new ArgumentException("User not found or could not be deleted.");
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
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            var userContext = this._userRepository.GetDbSet();

            var user = await userContext.Include(u => u.UserRole!).ThenInclude(r => r.RolePermissions!).SingleOrDefaultAsync(u=>u.UserId==userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            return user;
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
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(adminId)) throw new ArgumentNullException();

                var admin = await _userRepository.FindOneById(adminId);
                if (admin == null) throw new ArgumentException("admin does not exist");
                var user = await _userRepository.FindOneById(userId);
                if (user == null) throw new ArgumentException("user does not exist");

                user.UserRoleId = updateUserRoleDto.RoleId;
                await _userRepository.Update(user);
            }
            catch { throw; }
        }

        /// <summary>
        /// Deletes a user account by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user account to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when userId is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when user does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when user could not be deleted.</exception>
        public async Task DeleteUserAccount(string adminId,string userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(adminId)) throw new ArgumentNullException();

            var admin = await _userRepository.FindOneById(adminId);
            if (admin == null) throw new ArgumentException("admin does not exist");
            var user = await _userRepository.FindOneById(userId);
            if (user == null) throw new ArgumentException("user does not exist");

            var status = await _userRepository.Delete(user);
            if (status == 0)
            {
                throw new InvalidOperationException("User could not be deleted.");
            }

        }
    }
}

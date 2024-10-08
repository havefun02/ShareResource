using AutoMapper;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;


namespace ShareResource.Models
{
    public class Mapping:Profile
    {
        public Mapping() {
            CreateMap<User, UserResultDto>()
                .ForMember(m=>m.UserRole,opt=>opt
                    .MapFrom(src=>new RoleResultDto {RoleName=src.UserRoleId,UserPermissions=src.UserRole!.RolePermissions!.Select(rp =>new PermissionResultDto{Permission=rp.PermissionId!}).ToList()
                    } ));
            CreateMap<Role, RoleResultDto>()
               .ForMember(r => r.RoleName, opt => opt.MapFrom(src => src.RoleId)) // Map RoleId directly
               .ForMember(r => r.UserPermissions, opt => opt
                   .MapFrom(src => src.RolePermissions!.Select(rp => new PermissionResultDto
                   {
                       Permission = rp.PermissionId!
                   }).ToList() ?? new List<PermissionResultDto>())); // Handles null RolePermissions

        }
    }
}

using AutoMapper;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;


namespace ShareResource.Models
{
    public class Mapper:Profile
    {
        public Mapper() {
            CreateMap<User, RegisterResultDto>()
                .ForMember(m=>m.UserRole,opt=>opt
                    .MapFrom(src=>new RoleResultDto {RoleName=src.UserRoleId,UserPermissions=src.UserRole!.RolePermissions!.Select(rp =>new PermissionResultDto{Permission=rp.PermissionId!}).ToList()
                    } ));
        }
    }
}

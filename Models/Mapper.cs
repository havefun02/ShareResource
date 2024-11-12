using AutoMapper;
using CRUDFramework.Cores;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Models.ViewModels;


namespace ShareResource.Models
{
    public class Mapping:Profile
    {
        public Mapping() {
            CreateMap<Img, ImgResultViewModel>()
               .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User!.UserName))
               .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User!.UserEmail));
            CreateMap<ImgDto, Img>()
            .ForMember(dest => dest.ImgId, opt =>opt.MapFrom(src=>Guid.NewGuid().ToString())) // Assuming ImgId is generated and not in the DTO
            .ForMember(dest => dest.FileUrl, opt => opt.Ignore()) // Ignore if it's set elsewhere
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.file!.ContentType)) // Mapping ContentType from the file
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.file!.Length)) // Mapping file size
            .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set current date
            CreateMap<User, UserViewModel>();
            CreateMap<UpdateImgDto, Img>();
           


            CreateMap<Role, RoleResultDto>()
               .ForMember(r => r.Role, opt => opt.MapFrom(src => src.RoleName)) // Map RoleId directly
               .ForMember(r => r.Permissions, opt => opt
                   .MapFrom(src => src.RolePermissions!.Select(rp => new PermissionResultDto
                   {
                       Permission = rp.PermissionId!
                   }).ToList() ?? new List<PermissionResultDto>())); // Handles null RolePermissions
        }
    }
}

using AutoMapper;
using CRUDFramework.Cores;
using Microsoft.AspNetCore.Http;
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
               .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User!.UserEmail))
               .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.User!.UserId));

            CreateMap<ImgDto, Img>()
            .ForMember(dest => dest.ImgId, opt =>opt.MapFrom(src=>Guid.NewGuid().ToString())) // Assuming ImgId is generated and not in the DTO
            .ForMember(dest => dest.FileUrl, opt => opt.Ignore()) // Ignore if it's set elsewhere
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.file!.ContentType)) // Mapping ContentType from the file
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.file!.Length)) // Mapping file size
            .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set current date
            CreateMap<User, UserViewModel>();
            CreateMap<User, UserUpdateViewModel>()
                .ForMember(dest => dest.UserIconInBytes, opt => opt.MapFrom(src => src.UserIcon))
                .ForMember(dest => dest.UserIcon, opt => opt.Ignore());

            CreateMap<UserUpdateViewModel, UserDto>()
               .ForMember(dest => dest.UserIcon, opt => opt.MapFrom(src => ConvertFormFileToByteArray(src.UserIcon)));
            CreateMap<UpdateImgDto, Img>();
           


            CreateMap<Role, RoleResultDto>()
               .ForMember(r => r.Role, opt => opt.MapFrom(src => src.RoleName)) // Map RoleId directly
               .ForMember(r => r.Permissions, opt => opt
                   .MapFrom(src => src.RolePermissions!.Select(rp => new PermissionResultDto
                   {
                       Permission = rp.PermissionId!
                   }).ToList() ?? new List<PermissionResultDto>())); // Handles null RolePermissions
        }
        private byte[] ConvertFormFileToByteArray(IFormFile formFile)
        {
            if (formFile == null)
                return null;
            byte[] bytes;

            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                bytes= memoryStream.ToArray();
            }
            return bytes;
        }
    }
}

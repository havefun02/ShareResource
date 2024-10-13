using Microsoft.AspNetCore.Http;
namespace ShareResource.Models.Dtos
{
    public class ImgDto
    {
        public string FileName { get; set; } = string.Empty;
        public IFormFile? file {  get; set; }

    }
}

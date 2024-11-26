using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using System.IO;
using System.Text;

namespace ShareResource.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IUserService<User> _userService;
        private readonly IResourceReaderService<Img> _resourceService;

        public FileController(IUserService<User> userService, IResourceReaderService<Img> resourceService)
        {
            _userService = userService;
            _resourceService = resourceService;
        }
        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId )
        {
            var file = await _resourceService.GetResourceById(fileId);
            Console.WriteLine(file.FileUrl);
            var filePath="wwwroot"+file.FileUrl;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();  
            }
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

            string contentType = "application/octet-stream"; 
            string fileName = Path.GetFileName(filePath);
            return File(stream, contentType, fileName);
        }
    }

    //[HttpGet("file/donwload-ranges")]
    //    public async Task<IActionResult> DownloadFiles(string[] fileId)
    //    {


    //    }
    //}
}

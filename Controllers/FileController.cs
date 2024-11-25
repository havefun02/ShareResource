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
        public FileController(IUserService<User> userService)
        {
            _userService = userService;
        }
        [HttpGet("file/{userId}/{fileId}")]
        public async Task<IActionResult> DownloadFile( string userId,string fileId )
        {
            var file = await _userService.GetUserFile(userId, fileId);
            Console.WriteLine(file.FileUrl);
            var filePath="wwwroot"+file.FileUrl;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();  // File not found, return 404
            }
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

            string contentType = "application/octet-stream"; // Default binary file type
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

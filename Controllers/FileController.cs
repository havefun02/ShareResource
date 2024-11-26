using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using static System.Net.WebRequestMethods;

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
        [HttpGet("files/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var file = await _resourceService.GetResourceById(fileId);
            var filePath = "wwwroot" + file.FileUrl;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            string contentType = "application/octet-stream";
            string fileName = Path.GetFileName(filePath);
            return File(stream, contentType, fileName);
        }
        [HttpGet("files/donwload/{userId}")]
        public async Task<IActionResult> DownloadGallery(string userId)
        {
            var files = await _resourceService.GetUserResources(userId);
            if (files == null || !files.Any())
            {
                return NotFound("No files found for the user.");
            }
            var zipStream = new MemoryStream();
                using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        string filePath = Path.Combine("wwwroot", file.FileUrl);

                        if (!System.IO.File.Exists(filePath))
                            continue; 

                        string fileName = Path.GetFileName(filePath+".png");

                        var entry = archive.CreateEntry(fileName);
                        using var entryStream = entry.Open();
                        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

                        await fileStream.CopyToAsync(entryStream);
                    }
                }

                zipStream.Seek(0, SeekOrigin.Begin);
                return File(zipStream, "application/zip", "Gallery.zip");
        }
    }
}

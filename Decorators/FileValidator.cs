using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShareResource.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShareResource.Decorators
{
    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FileValidator: ActionFilterAttribute
    {
        private const long _maxFileSize = 5 * 1024* 1024; // 5MB limit
        private readonly string[] AllowedMimeTypes = { "image/png", "image/jpeg", "video/mp4" };
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".mp4" };
        public FileValidator() { }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("fileMeta", out var fileObj) && fileObj is ImgDto file)
            {
                // Check file extension
                var extension = Path.GetExtension(file.file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    context.Result = new BadRequestObjectResult($"Invalid file type. Allowed types: {string.Join(", ", _allowedExtensions)}");
                    return;
                }

                // Check file size
                if (file.file!.Length > _maxFileSize)
                {
                    context.Result = new BadRequestObjectResult($"File size must not exceed {_maxFileSize / (1024 * 1024)} MB.");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("File is required.");
            }
        }
    }
}

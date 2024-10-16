using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using ShareResource.Decorators;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public class FileValidationMiddleware
{
    private readonly RequestDelegate _next;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB limit
    private readonly string[] AllowedMimeTypes = { "image/png", "image/jpeg", "video/mp4" };
    private readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".mp4" };

    public FileValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldValidateFileUpload(context))
        {
            var file = context.Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("No file uploaded.");
                return;
            }

            if (file.Length > MaxFileSize)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("File size exceeds the allowed limit (5MB).");
                return;
            }

            if (!AllowedMimeTypes.Contains(file.ContentType))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid file type.");
                return;
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid file extension.");
                return;
            }
        }

        await _next(context);
    }

    private bool ShouldValidateFileUpload(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null) return false;

        var controllerActionDescriptor = endpoint.Metadata
            .GetMetadata<ControllerActionDescriptor>();

        return controllerActionDescriptor?.MethodInfo
            .GetCustomAttributes(typeof(FileValidator), true)
            .Any() ?? false;
    }
}

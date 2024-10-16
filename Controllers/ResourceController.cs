using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;
using ShareResource.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Claims;
using ShareResource.Models.ViewModels;
using ShareResource.Decorators;


namespace ShareResource.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IResource<Img> _service;
        private readonly string _imageFolderPath;


        public ResourceController(IResource<Img> service, IMapper mapper,IConfiguration configuration)
        {
            _service = service;
            _imageFolderPath = configuration.GetValue<string>("ImageStorage:ImageFolderPath") ?? string.Empty;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Main()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found. Please log in.";
                return Forbid("You are not allowed to access page");
            }
            var resources = await _service.GetDetailAllResource(userId);
            var resourceViewModel = _mapper.Map<List<ImgResult>>(resources);
            //var imgDto = new ImgDto();
            //var model = Tuple.Create(resourceViewModel.AsEnumerable(), imgDto);
            return View(resourceViewModel);
        }

        /// <summary>
        /// Upload a new image resource.
        /// </summary>
        /// <param name="resource">The image resource to upload.</param>
        /// <returns>The uploaded image resource.</returns>
        ///   [HttpPost]
        ///   
        [HttpPost]
        [Authorize]
        [FileValidator]
        public async Task<IActionResult> Upload([FromForm] ImgDto fileMeta)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            // Check if user ID is valid
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found. Please log in.";
                return BadRequest("");
            }

            var fileExtension = Path.GetExtension(fileMeta.file!.FileName).ToLowerInvariant();

            

            // Check model state validity
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid model data.";
                return BadRequest();
            }

            try
            {
                var folderPath = _imageFolderPath+"/"+ userId; // You might want to use a unique filename
                Directory.CreateDirectory(("wwwroot"+ folderPath));
                var filePath=folderPath +"/"+ Guid.NewGuid().ToString() + fileMeta.FileName + fileExtension;
                using (var stream = new FileStream("wwwroot"+filePath, FileMode.Create))
                {
                    await fileMeta.file.CopyToAsync(stream); // Save the file to the server
                }
                var imgToUpload = _mapper.Map<Img>(fileMeta);
                imgToUpload.FileUrl = filePath;
                var uploadedResource = await _service.UploadResource(imgToUpload, userId);
                var displayData=_mapper.Map<ImgResult>(uploadedResource);
                TempData["Success"] = "File uploaded successfully!";
                return PartialView("_ImgItem",displayData); // You can redirect to another action if needed

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading file: {ex.Message}";
                return BadRequest();
            }
        }


        /// <summary>
        /// Edit an existing image resource.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to update.</param>
        /// <param name="resource">The updated image resource.</param>
        /// <returns>The updated image resource.</returns>
        [HttpPut("{resourceId}")]
        [Authorize()] // Adjust the policy as needed
        public async Task<IActionResult> EditResource(string resourceId, [FromBody] UpdateImgDto resource)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid resource.");
            }

            try
            {
                var imgToUpdate = _mapper.Map<Img>(resource);
                var updatedResource = await _service.EditResource(imgToUpdate, userId);
                return Ok(updatedResource);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error editing resource: {ex.Message}");
            }
        }

        /// <summary>
        /// Get details of a specific image resource by ID.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to retrieve.</param>
        /// <returns>The image resource.</returns>
        [HttpGet("{resourceId}")]
        [Authorize] // Adjust the policy as needed
        public async Task<IActionResult> GetDetailResourceById(string resourceId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            try
            {
                
                var resourceResult = await _service.GetDetailResourceById(userId, resourceId);
                if (resourceResult == null)
                {
                    return NotFound("Resource not found.");
                }
                return Ok(resourceResult);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving resource: {ex.Message}");
            }
        }


        /// <summary>
        /// Delete a specific image resource by ID.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        [HttpDelete("{resourceId}")]
        [Authorize()] // Only admin can delete
        public async Task<IActionResult> DeleteResource(string resourceId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            try
            {
                var result = await _service.DeleteResource(resourceId, userId);
                if (result == 0) // Assuming 0 indicates failure to delete
                {
                    return NotFound("Resource not found or cannot be deleted.");
                }

                return NoContent(); // Successful deletion
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting resource: {ex.Message}");
            }
        }
    }
}

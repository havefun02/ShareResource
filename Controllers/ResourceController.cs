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
using CRUDFramework.Cores;
using Microsoft.EntityFrameworkCore.Query;
using NUglify;


namespace ShareResource.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IResourceWriterService<Img> _service;
        private readonly IResourceReaderService<Img> _accessService;
        private readonly IUserService<User> _userService;

        private readonly string _imageFolderPath;
        public ResourceController(IResourceWriterService<Img> service, IResourceReaderService<Img> accessService, IMapper mapper, IConfiguration configuration, IUserService<User> userService)
        {
            _accessService=accessService;
            _userService = userService;
            _service = service;
            _imageFolderPath = configuration.GetValue<string>("ImageStorage:ImageFolderPath") ?? string.Empty;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Resource([FromQuery] int page = 1)
        {
            if (page <= 0)
            {
                var mainView = new MainPageViewModel { Gallery = null, User = null};
                return View(mainView);
            }
            else
            {
                OffsetPaginationParams offsetParams = new OffsetPaginationParams();
                offsetParams.offset = (page - 1) * offsetParams.limit;
                var resources = await _accessService.GetSampleResource(offsetParams) as OffsetPaginationResult<Img>;
                var resourceViewModel = _mapper.Map<List<ImgResultViewModel>>(resources!.items);
                var paginationModel = new PaginationViewModel { limit = resources.limit, offset = resources.offset, currentPage = resources.offset / resources.limit + 1, totalItems = resources.totalItems };
                var galleryViewModel=new GalleryViewModel() { Imgs= resourceViewModel,Pagination= paginationModel };
                var mainView = new MainPageViewModel { User = null, Gallery = galleryViewModel };
                return View(mainView);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserResource([FromQuery] int page)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found. Please log in.";
                return Redirect("/api/v1/auths/login");
            }
            if (page <= 0) { 
                TempData["Error"] = "Invalid request";
                return NoContent();
            }
            else
            {
                OffsetPaginationParams offsetParams = new OffsetPaginationParams();
                offsetParams.offset = (page - 1) * offsetParams.limit;
                var resources = await _accessService.GetUserResources(offsetParams,userId) as OffsetPaginationResult<Img>;
                var resourceViewModel = _mapper.Map<List<ImgResultViewModel>>(resources!.items);
                var paginationViewModel = new PaginationViewModel() { limit = resources.limit, offset = resources.offset, totalItems = resources.totalItems, currentPage = (int)Math.Ceiling((decimal)resources.offset / resources.limit) };
                var galleryViewModel = new GalleryViewModel() { Imgs = resourceViewModel, Pagination = paginationViewModel };
                return PartialView("_UserGallery", galleryViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAppData([FromQuery] int page)
        {
            if (page <= 0)
            {
                return BadRequest("Invalid request");
            }
            else
            {
                OffsetPaginationParams offsetParams = new OffsetPaginationParams();
                offsetParams.offset = (page - 1) * offsetParams.limit;
                var resources = await _accessService.GetSampleResource(offsetParams) as OffsetPaginationResult<Img>;
                var resourceViewModel = _mapper.Map<List<ImgResultViewModel>>(resources!.items);
                return new JsonResult(resourceViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserProfile(string userId, int page)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found.";
                return BadRequest("You are not allowed to access page");
            }
            OffsetPaginationParams offsetParams = new OffsetPaginationParams();
            if (offsetParams?.offset == null)
            {
                offsetParams!.offset = 1;
            }
            else
            {
                offsetParams.offset = page * offsetParams.limit;
            }

            var userResources = await _accessService.GetPublicUserResources(offsetParams,userId) as OffsetPaginationResult<Img>;
            var resourceViewModel = _mapper.Map<List<ImgResultViewModel>>(userResources!.items);
            var userProfile = await _userService.GetUserProfile(userId);
            var userViewModel = _mapper.Map<UserViewModel>(userProfile);
            var paginationModel = new PaginationViewModel { limit = userResources.limit, offset = userResources.offset, currentPage = userResources.offset / userResources.limit + 1, totalItems = userResources.totalItems };
            var galleryViewModel = new GalleryViewModel() { Imgs = resourceViewModel, Pagination = paginationModel };

            var mainView = new MainPageViewModel { User = userViewModel ,Gallery= galleryViewModel };
            return View("UserProfile", mainView);
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
        public async Task<IActionResult> Upload(ImgDto fileMeta)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found. Please log in.";
                return Redirect("/");            }

            var fileExtension = Path.GetExtension(fileMeta.file!.FileName).ToLowerInvariant();


            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid model data.";
                return RedirectToAction("GetUserProfile","User");
            }

            try
            {
                var folderPath = _imageFolderPath + "/" + userId; // You might want to use a unique filename
                Directory.CreateDirectory("wwwroot" + folderPath);
                var filePath = folderPath + "/" + Guid.NewGuid().ToString() + fileMeta.FileName + fileExtension;
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    await fileMeta.file.CopyToAsync(stream); // Save the file to the server
                }
                var imgToUpload = _mapper.Map<Img>(fileMeta);
                imgToUpload.FileUrl = filePath;
                var uploadedResource = await _service.UploadResource(imgToUpload, userId);
                TempData["Success"] = "Created success.";

                return RedirectToAction("GetUserProfile", "User");

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading file: {ex.Message}";
                return RedirectToAction("GetUserProfile", "User");
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadChunk([FromForm] VidDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User not found. Please log in.";
                return BadRequest(TempData["Error"]);
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid model or content range");

                TempData["Error"] = "Invalid model data.";
                return BadRequest(TempData["Error"]);
            }

            if (dto.source == null)
            {
                Console.WriteLine("Invalid chunk or content range");

                return BadRequest("Invalid chunk or content range");
            }

            var folderPath = _imageFolderPath + "/" + userId+"/videos"; // You might want to use a unique filename

            Directory.CreateDirectory("wwwroot/" + folderPath);
            var filePath = "wwwroot" + folderPath + "/" + dto.FileName + ".mp4";

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                stream.Seek(dto.start, SeekOrigin.Begin);
                await dto.source.CopyToAsync(stream);
            }
            return Ok("Chunk uploaded");
        }


        /// <summary>
        /// Edit an existing image resource.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to update.</param>
        /// <param name="resource">The updated image resource.</param>
        /// <returns>The updated image resource.</returns>
        [Authorize] // Adjust the policy as needed
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
                imgToUpdate.ImgId = resourceId;
                var updatedResource = await _service.EditResource(imgToUpdate, userId);
                return Ok(new {redirectUrl="/resources/profile" });
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
        public async Task<IActionResult> GetDetailResourceById(string resourceId)
        {
            try
            {

                var resourceResult = await _accessService.GetResourceById(resourceId);
                if (resourceResult == null)
                {
                    return NotFound("Resource not found.");
                }
                var displayData = _mapper.Map<ImgResultViewModel>(resourceResult);

                return View("SingleItem", displayData);

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
        [Authorize] // Only admin can delete
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

                return Ok(new { redirectUrl = "/resources/profile" });

            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting resource: {ex.Message}");
            }
        }
    }
}

using AutoMapper;
using CRUDFramework.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Models.ViewModels;
using ShareResource.Services;
using System.Security.Claims;

namespace ShareResource.Controllers
{
    [Route("/")]
    public class UserController : Controller
    {
        private readonly IUserService<User> _userService;
        private readonly IResourceReaderService<Img> _resourceService;

        private readonly IMapper _mapper;
        public UserController(IUserService<User> userService, IMapper mapper, IResourceReaderService<Img> resourceService)
        {
            _mapper = mapper;
            _userService = userService;
            _resourceService = resourceService;
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                ViewData["Error"] = "User ID cannot be null or empty.";
                return View("Main");
            }
            try
            {
                var user = await _userService.GetUserProfile(userId);
                var userViewModel = _mapper.Map<UserViewModel>(user);
                var offsetParams=new OffsetPaginationParams { limit = 12,offset=1 };
                var resource = await _resourceService.GetUserResources(offsetParams,userId) as OffsetPaginationResult<Img> ?? null;
                if (resource == null) {
                    return View("Main");
                }
                var paginationViewModel=new PaginationViewModel() {limit=resource.limit,offset=resource.offset,totalItems=resource.totalItems,currentPage=(int)Math.Ceiling((decimal) resource.offset/resource.limit)};
                var imgViewModel = _mapper.Map<List<Img>,List<ImgResultViewModel>>(resource.items!);
                var mainViewModel = new MainPageViewModel() { User = userViewModel, Pagination = paginationViewModel, Imgs =imgViewModel };
                return View("Main",mainViewModel);
            }
            catch (Exception)
            {
                return View("Main");
            }
        }
        [Authorize]
        [HttpPut("basic-users")]
        public async Task<ActionResult<UserResultDto>> EditProfile([FromBody] UserDto userDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (userDto == null || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User data cannot be null.");
            }
            try
            {
                var updatedUser = await _userService.EditProfile(userDto, userId);
                var userResult = _mapper.Map<UserResultDto>(updatedUser);
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("basic-users")]
        public async Task<IActionResult> DeleteProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                await _userService.DeleteProfile(userId);
                HttpContext.Response.Cookies.Delete("accessToken");
                HttpContext.Response.Cookies.Delete("refreshToken");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

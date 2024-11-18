using AutoMapper;
using CRUDFramework;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Models.ViewModels;
using ShareResource.Services;
using System.ComponentModel.Design;
using System.Security.Claims;
namespace ShareResource.Controllers
{
    public class HomeController:Controller
    {
        private readonly IResourceReaderService<Img> _accessService;
        private readonly IMapper _mapper;
        public HomeController(IResourceReaderService<Img> accessService,IMapper mapper)
        {
            _accessService = accessService;
            _mapper = mapper;
        }
        [HttpGet("about")]
        public IActionResult About()
        {
            var aboutViewModel = new AboutViewModel { Email = "example@gmail.com", Address = "3, N street, Ny city", PhoneNumber = "+84 2233445566" };
            return View("About",aboutViewModel);
        }

        [HttpGet("explore")]
        public async Task<IActionResult> Explore(int page=1)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                if (page <= 0)
                {
                    TempData["Error"] = "Invalid request";
                    return Redirect("/explore");
                }
                else
                {
                    OffsetPaginationParams offsetParams = new OffsetPaginationParams();
                    offsetParams.offset = (page - 1) * offsetParams.limit;
                    var resources = await _accessService.GetSampleResource(offsetParams) as OffsetPaginationResult<Img>?? null;
                    var paginationViewModel = new PaginationViewModel() { limit = resources.limit, offset = resources.offset, totalItems = resources.totalItems, currentPage = (int)Math.Ceiling((decimal)resources.offset / resources.limit) + 1 };
                    var imgViewModel = _mapper.Map<List<Img>, List<ImgResultViewModel>>(resources.items!);
                    if (!string.IsNullOrEmpty(userId) && User.Identity.IsAuthenticated)
                    {
                        var index = 0;
                        resources.items.ForEach(t => {
                            if (t.ImgLovers.Where(t => t.UserId == userId).Any())
                            {
                                imgViewModel[index].LikeState = true;
                            }
                            else
                            {
                                imgViewModel[index].LikeState = false;
                            }
                            index++;
                            });
                    }
                    var galleryViewModel = new GalleryViewModel() { Imgs = imgViewModel, Pagination = paginationViewModel };
                    return View("Explore", galleryViewModel);
                }
            }
            catch (Exception)
            {
                return View("Explore");
            }
        }

    }
}

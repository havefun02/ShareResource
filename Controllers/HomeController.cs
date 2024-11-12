using AutoMapper;
using CRUDFramework.Cores;
using Microsoft.AspNetCore.Mvc;
using ShareResource.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;
using ShareResource.Models.ViewModels;
using System.ComponentModel.Design;
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
        public async Task<IActionResult> Explore()
        {
            var offsetParams = new OffsetPaginationParams { limit = 12, offset = 1 };
            var resource = await _accessService.GetSampleResource(offsetParams) as OffsetPaginationResult<Img> ?? null;
            if (resource == null)
            {
                return View("Explore",null);
            }
            var paginationViewModel = new PaginationViewModel() { limit = resource.limit, offset = resource.offset, totalItems = resource.totalItems, currentPage = (int)Math.Ceiling((decimal)resource.offset / resource.limit) };
            var imgViewModel = _mapper.Map<List<Img>, List<ImgResultViewModel>>(resource.items!);
            var mainViewModel = new MainPageViewModel() { User = null, Pagination = paginationViewModel, Imgs = imgViewModel };

            return View("Explore", mainViewModel);
        }
    }
}

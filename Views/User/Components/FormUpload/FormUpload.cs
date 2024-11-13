using Microsoft.AspNetCore.Mvc;
using ShareResource.Models.Dtos;
namespace ShareResource.Views.User.Components.FormUpload
{
    public class FormUploadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var dto = new ImgDto();
            return View(dto);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ShareResource.Models.Dtos;
using ShareResource.Models.ViewModels;

namespace ShareResource.Views.Shared.Components.LoginDataComponent
{
    public class LoginDataViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var user = HttpContext.User;
            var username = user.Identity.Name;

            var dto = new LoginDataViewModel() { UserName=username};
            return View(dto);
        }
    }
}

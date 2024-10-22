using Microsoft.AspNetCore.Mvc;
using ShareResource.Models.Dtos;
using ShareResource.Models.ViewModels;
namespace ShareResource.Controllers
{
    public class HomeController:Controller
    {
        [HttpGet("about")]
        public IActionResult About()
        {
            var aboutViewModel = new AboutViewModel { Email = "example@gmail.com", Address = "3, N street, Ny city", PhoneNumber = "+84 2233445566" };
            return View("About",aboutViewModel);
        }

        [HttpGet("/account/login")]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpGet("/account/register")]
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpGet("/account/change-password")]
        public  IActionResult ChangePassword()
        {
            return View("ResetPassword");
        }
    }
}

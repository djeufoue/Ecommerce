
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            // 1. use the information inside 'register' to register the user
            //register.FirstName

            // 4. Redirect user to the Login page
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {

            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel Login)
        {

            return LocalRedirect("/");
        }


        public IActionResult Logout()
        {
            return LocalRedirect("/");
        }
    }
}

using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly EcommerceDbContext _context;

        public AccountController(
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger,
            EcommerceDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // 1. use the information inside 'register' to register the user
            string email = model.Email;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(model.Email), $"User account {email} already exists.");
                return View(model);
            }
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    throw new ApplicationException("SendConfirmationEmail(user) is not implemented");
                }
                _logger.LogInformation($"User account {email} created successfully.");
                // 4. Redirect user to the Login page
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var errorsStr = JsonConvert.SerializeObject(result.Errors);
                var modelStr = JsonConvert.SerializeObject(model);
                _logger.LogWarning($"Failed to Register. userDto: {modelStr} errors: {errorsStr}");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(model.Password), error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, [FromQuery] string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            string email = model.Email;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), $"User account {email} not found.");
                return View(model);
            }

            if (_userManager.Options.SignIn.RequireConfirmedAccount && !user.EmailConfirmed)
            {
                throw new ApplicationException("SendConfirmationEmail(user) is not implemented");
            }

            var result = await _signInManager.PasswordSignInAsync(email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User account {email} has logged in.");

                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = "/";

                return LocalRedirect(returnUrl);
            }
            else if (result.RequiresTwoFactor)
            {
                throw new ApplicationException("Two Factor Authentication is not implemented");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(nameof(model.Email), $"User account {email} is locked out.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(nameof(model.Password), $"The provided password is not correct.");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            //HttpContext.Session.Clear();

            foreach (var cookie in HttpContext.Request.Cookies)
                Response.Cookies.Delete(cookie.Key);

            _logger.LogInformation($"User {User.Identity.Name} logged out.");
            return LocalRedirect("/");
        }
    }
}

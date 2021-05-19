using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        // _context here is how dartabase 
        private EcommerceDbContext _context;

        // constructor 
        public ProductController(EcommerceDbContext context)
        {

            _context = context;
        }

        [HttpGet]
        public IActionResult ViewProduct(long id)
        {

            //  
            Product product = _context.Products
                                    .Where(x => x.Id == id)
                                    .FirstOrDefault();
            // this is actualy the code that allow the image and the information about a product to be view when we clicked on a
            // product in the main page. Allow us to get all the values from the database.

            var ViewModel = new ProductViewModel();
            ViewModel.Title = product.Title;
            ViewModel.Description = product.Description;
            ViewModel.Price = product.Price;
            ViewModel.Quantity = product.Quantity;
            ViewModel.CreatedAt = product.CreatedAt;
            ViewModel.publishedAt = product.publishedAt;
            ViewModel.ImageURL = product.ImageURL;

            return View(ViewModel);
        }

        // this "[Authorize]" means that this page can only be access by those who have already login
        [Authorize]
        [HttpGet]
        public IActionResult AddProduct()
        {
            var ViewModel = new ProductViewModel();
            return View(ViewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddProduct(ProductViewModel model)
        {
            Product product = new Product();
            product.Title = model.Title;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.CreatedAt = model.CreatedAt;
            product.publishedAt = model.publishedAt;

            // User.Identity.Name is the UserName of the logged-in user
            ApplicationUser user = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();
            product.UserId = user.Id;

            // add product to database (does not save database)
            _context.Products.Add(product);

            // save database (will write all changes to the database)
            _context.SaveChanges();

            return LocalRedirect("/");
        }
    }
}

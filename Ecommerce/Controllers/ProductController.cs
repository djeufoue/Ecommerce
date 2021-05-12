using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {

        private EcommerceDbContext _context;

        public ProductController(EcommerceDbContext context) {

            _context = context;
        }


        [HttpGet]
        public IActionResult ViewProduct(long id)
        {
            Product product = _context.Products
                                    .Where(x => x.Id == id)
                                    .FirstOrDefault();

            var ViewModel = new ProductViewModel();
            ViewModel.Title = product.Title;
            ViewModel.Description = product.Description;
            ViewModel.Price = product.Price;
            ViewModel.Quantity = product.Quantity;
            ViewModel.CreatedAt = product.CreatedAt;
            ViewModel.publishedAt = product.publishedAt;

            return View(ViewModel);
        }


        [HttpGet]
        public IActionResult AddProduct()
        {

            var ViewModel = new ProductViewModel();
            return View(ViewModel);
        }

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


            // add product to database (does not save database)
            _context.Products.Add(product);

            // save database (will write all changes to the database)
            _context.SaveChanges();

            return LocalRedirect("/");
        }

    }
}

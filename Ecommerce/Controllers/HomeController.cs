
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EcommerceDbContext _context;

        public HomeController(ILogger<HomeController> logger, EcommerceDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchString = null)
        {
            List<Product> products = _context.Products.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Title.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TextPage() {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

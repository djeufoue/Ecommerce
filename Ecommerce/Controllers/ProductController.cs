using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceDbContext _context;

        public ProductController(EcommerceDbContext context)
        {
            _context = context;
        }

        private ProductViewModel GetViewModelFromModel(Product product)
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Quantity = product.Quantity,
                CreatedAt = product.CreatedAt,
                publishedAt = product.publishedAt,
                Description = product.Description,
                ImageURL = product.ImageURL,
            };
            return productViewModel;
        }

        private Product GetModelFromViewModel(ProductViewModel p)
        {
            Product product = new Product
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Quantity = p.Quantity,
                CreatedAt = p.CreatedAt,
                publishedAt = p.publishedAt,
                Description = p.Description,
                ImageURL = p.ImageURL,
            };
            return product;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.ToListAsync();
            List<ProductViewModel> productViewModels = new List<ProductViewModel>();
            foreach (Product product in products)
            {
                productViewModels.Add(GetViewModelFromModel(product));
            }
            return View(productViewModels);
        }

        // GET: Product/Details/5
        public IActionResult Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = GetViewModelFromModel(product);

            return View(productViewModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Title,Description,Price,Quantity,CreatedAt,publishedAt,ImageURL,Phone")] ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = GetModelFromViewModel(productViewModel);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Product/Edit/5
        public IActionResult Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = GetViewModelFromModel(product);

            return View(productViewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,Title,Description,Price,Quantity,CreatedAt,publishedAt,ImageURL,Phone")] ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Product product = GetModelFromViewModel(productViewModel);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_context.Products.Find(id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Product/Delete/5
        public IActionResult Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = GetViewModelFromModel(product);

            return View(productViewModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

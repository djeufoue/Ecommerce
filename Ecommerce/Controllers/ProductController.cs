using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly IConfiguration _configuration;

        public ProductController(EcommerceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private ProductViewModel GetViewModelFromModel(Product product)
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Id = product.Id,
                UserId = product.UserId,
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
                UserId = p.UserId,
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
        [AllowAnonymous]
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

        [AllowAnonymous]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string filePathName = _configuration.GetValue<string>("PathToProductsImages") + fileName;

                    if (System.IO.File.Exists(filePathName))
                    {
                        var modificationDate = System.IO.File.GetLastWriteTimeUtc(filePathName);
                        FileStream fileStream = System.IO.File.OpenRead(filePathName);
                        string contentType = MimeTypes.GetMimeType(fileName);

                        var contentDisposition = new System.Net.Mime.ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true,
                            Size = fileStream.Length,
                            ModificationDate = modificationDate
                        };
                        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                        return File(fileStream, contentType);
                    }
                    else return NotFound($"File '{fileName}' not found");
                }
                else return BadRequest("File name not provided");
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private async Task SaveImageFile(IFormFile formFile, Product product)
        {
            if (formFile != null)
            {
                string folder = _configuration.GetValue<string>("PathToProductsImages");
                string dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
                string fileName = $"Product {dateTime} {formFile.FileName}";

                using (FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(fileStream);
                }
                product.ImageURL = $"/Product/{nameof(GetFile)}?fileName={fileName}";
            }
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
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = GetModelFromViewModel(productViewModel);

                ApplicationUser user = _context.Users
                                            .Where(u => u.UserName == User.Identity.Name)
                                            .First();
                product.UserId = user.Id;

                await SaveImageFile(productViewModel.ImageFile, product);

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

            ApplicationUser user = _context.Users
                                        .Where(u => u.UserName == User.Identity.Name)
                                        .First();
            if (product.UserId != user.Id)
                return Forbid("Not a chance in hell!");

            var productViewModel = GetViewModelFromModel(product);

            return View(productViewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProductViewModel productViewModel)
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

                    ApplicationUser user = _context.Users
                                                .Where(u => u.UserName == User.Identity.Name)
                                                .First();
                    if (product.UserId != user.Id)
                        return Forbid("Not a chance in hell!");

                    await SaveImageFile(productViewModel.ImageFile, product);

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

            ApplicationUser user = _context.Users
                                        .Where(u => u.UserName == User.Identity.Name)
                                        .First();
            if (product.UserId != user.Id)
                return Forbid("Not a chance in hell!");

            var productViewModel = GetViewModelFromModel(product);

            return View(productViewModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            Product product = await _context.Products.FindAsync(id);

            ApplicationUser user = _context.Users
                                        .Where(u => u.UserName == User.Identity.Name)
                                        .First();
            if (product.UserId != user.Id)
                return Forbid("Not a chance in hell!");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

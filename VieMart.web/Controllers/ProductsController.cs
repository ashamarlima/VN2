using System;
using System.IO;
using System.Linq;                             // OrderByDescending
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;            // IWebHostEnvironment
using VieMart.web.Services;
using VieMart.web.Models;

namespace VieMart.web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Product image is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // save the image file (timestamp + original extension) to wwwroot/products
            string? newFileName = null;
            if (productDto.ImageFile != null)
            {
                var folder = Path.Combine(environment.WebRootPath, "products");
                Directory.CreateDirectory(folder);

                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                              Path.GetExtension(productDto.ImageFile.FileName);

                var imageFullPath = Path.Combine(folder, newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
            }

            // map DTO -> entity
            var product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Price = productDto.Price,
                Category = productDto.Category,
                Description = productDto.Description,
                ImageFileName = newFileName ?? "noimage.png",
                CreatedAt = DateTime.UtcNow
            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        // -------- EDIT (GET) --------
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto
            {
                Name = product.Name,
                Brand = product.Brand,
                Price = product.Price,
                Category = product.Category,
                Description = product.Description
            };

            ViewData["ProductId"] = id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt;

            return View(productDto);
        }

        // -------- EDIT (POST) --------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductDto dto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt;
                return View(dto);
            }

            // update simple fields
            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Description = dto.Description;

            // replace image only if a new file was uploaded
            if (dto.ImageFile != null)
            {
                var folder = Path.Combine(environment.WebRootPath, "products");
                Directory.CreateDirectory(folder);

                // delete old file if present
                if (!string.IsNullOrWhiteSpace(product.ImageFileName))
                {
                    var oldPath = Path.Combine(folder, product.ImageFileName);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                  Path.GetExtension(dto.ImageFile.FileName);

                var newPath = Path.Combine(folder, newFileName);
                using var stream = System.IO.File.Create(newPath);
                dto.ImageFile.CopyTo(stream);

                product.ImageFileName = newFileName;
            }

            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        // -------- DELETE (immediate) --------
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // delete image file if exists
            if (!string.IsNullOrWhiteSpace(product.ImageFileName))
            {
                var imageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
                if (System.IO.File.Exists(imageFullPath))
                {
                    System.IO.File.Delete(imageFullPath);
                }
            }

            context.Products.Remove(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }
    }
}

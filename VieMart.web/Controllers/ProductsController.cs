using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using VieMart.web.Models;
using VieMart.web.Services;

namespace VieMart.web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IValidator<ProductDto> _validator;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env, IValidator<ProductDto> validator)
        {
            _context = context;
            _env = env;
            _validator = validator;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        // ---------- Create ----------
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductDto dto)
        {
            // validate with Create ruleset
            ValidationResult vr = _validator.Validate(dto, o => o.IncludeRuleSets("Create"));
            if (!vr.IsValid)
            {
                vr.AddToModelState(ModelState, null);
                return View(dto);
            }

            // save image
            string? fileName = null;
            if (dto.ImageFile != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "products");
                Directory.CreateDirectory(folder);

                fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                           Path.GetExtension(dto.ImageFile.FileName);

                using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
                dto.ImageFile.CopyTo(stream);
            }

            var entity = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Price = dto.Price,
                Category = dto.Category,
                Description = dto.Description,
                ImageFileName = fileName ?? "noimage.png",
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(entity);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ---------- Edit ----------
        public IActionResult Edit(int id)
        {
            var p = _context.Products.Find(id);
            if (p == null) return RedirectToAction(nameof(Index));

            var dto = new ProductDto
            {
                Name = p.Name,
                Brand = p.Brand,
                Price = p.Price,
                Category = p.Category,
                Description = p.Description
            };
            ViewData["ProductId"] = id;
            ViewData["ImageFileName"] = p.ImageFileName;
            ViewData["CreatedAt"] = p.CreatedAt;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductDto dto)
        {
            var product = _context.Products.Find(id);
            if (product == null) return RedirectToAction(nameof(Index));

            // validate with Edit ruleset
            var vr = _validator.Validate(dto, o => o.IncludeRuleSets("Edit"));
            if (!vr.IsValid)
            {
                vr.AddToModelState(ModelState, null);
                ViewData["ProductId"] = id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt;
                return View(dto);
            }

            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Description = dto.Description;

            // replace image only if new one uploaded
            if (dto.ImageFile != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "products");
                Directory.CreateDirectory(folder);

                // delete old
                if (!string.IsNullOrWhiteSpace(product.ImageFileName))
                {
                    var oldPath = Path.Combine(folder, product.ImageFileName);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                  Path.GetExtension(dto.ImageFile.FileName);

                using var stream = System.IO.File.Create(Path.Combine(folder, newFileName));
                dto.ImageFile.CopyTo(stream);

                product.ImageFileName = newFileName;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

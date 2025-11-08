using System.Diagnostics;
using System.Linq;                   
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VieMart.web.Models;
using VieMart.web.Services;         

namespace VieMart.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;   // <-- add
         
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // /Home/Index?q=iphone&category=Phones&minPrice=100&maxPrice=500
        [HttpGet]
        public IActionResult Index([FromQuery] ProductBrowseVm vm)
        {
            // categories for dropdown
            vm.Categories = _context.Products
                                    .Select(p => p.Category)
                                    .Distinct()
                                    .OrderBy(c => c)
                                    .ToList();

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Q))
            {
                var q = vm.Q.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(q) ||
                    p.Brand.ToLower().Contains(q) ||
                    p.Description.ToLower().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(vm.Category) && vm.Category != "All")
                query = query.Where(p => p.Category == vm.Category);

            if (vm.MinPrice.HasValue)
                query = query.Where(p => p.Price >= vm.MinPrice.Value);

            if (vm.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= vm.MaxPrice.Value);

            vm.Results = query.OrderByDescending(p => p.Id).ToList();
            return View(vm);   // pass the viewmodel
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

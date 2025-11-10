using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieMart.web.Services;

namespace VieMart.web.Controllers
{
    [Route("r")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RedirectController(ApplicationDbContext context) => _context = context;

        // Use built-in length constraints (no regex parser issues)
        [HttpGet("{shortCode:minlength(4):maxlength(10)}")]
        public async Task<IActionResult> GetRedirect(string shortCode)
        {
            // ensure alphanumeric so we don't collide with MVC paths
            if (!shortCode.All(char.IsLetterOrDigit))
                return NotFound();

            var urlEntry = await _context.ShortenedUrls
                                         .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (urlEntry == null) return NotFound();

            return Redirect(urlEntry.OriginalUrl); // 302
        }
    }
}

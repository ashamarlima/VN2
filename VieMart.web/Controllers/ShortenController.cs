using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieMart.web.Models;
using VieMart.web.Services;

namespace VieMart.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShortenController(ApplicationDbContext context) => _context = context;

        public class ShortenRequest { public string Url { get; set; } = string.Empty; }

        [HttpPost]
        public async Task<IActionResult> PostShortenUrl([FromBody] ShortenRequest request)
        {
            if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
                return BadRequest("Invalid URL provided.");

            string shortCode;
            do
            {
                shortCode = Guid.NewGuid().ToString("N")[..6];
            } while (await _context.ShortenedUrls.AnyAsync(x => x.ShortCode == shortCode));

            var shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = request.Url,
                ShortCode = shortCode,
                DateCreated = DateTime.UtcNow
            };

            _context.ShortenedUrls.Add(shortenedUrl);
            await _context.SaveChangesAsync();

            var shortUrl = $"{Request.Scheme}://{Request.Host}/r/{shortCode}";
            return Ok(new { ShortUrl = shortUrl });
        }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;    

namespace VieMart.web.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
    }
}

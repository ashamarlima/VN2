using System;
using System.ComponentModel.DataAnnotations;

namespace VieMart.web.Models
{
    public class ShortenedUrl
    {
        public int Id { get; set; }

        [Required, MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}

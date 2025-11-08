using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace VieMart.web.Models    
{
    public class Product
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }   = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string Description { get; set; } = "";

        [Required, MaxLength(100)]
        public string Category { get; set; } = "";

        [MaxLength(255)]
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; }  
    }
}

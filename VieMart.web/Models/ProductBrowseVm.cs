using System.Collections.Generic;

namespace VieMart.web.Models
{
    public class ProductBrowseVm
    {
 
        public string? Q { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        
        public List<string> Categories { get; set; } = new();
        public List<Product> Results { get; set; } = new();
    }
}

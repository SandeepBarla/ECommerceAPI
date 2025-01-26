using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.RequestModels
{
    public class ProductCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; } // Not required

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } // Optional

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class OrderProduct
    {
        [Key] // ✅ Composite Key (OrderId + ProductId)
        public int OrderId { get; set; }  

        [Key] 
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // ✅ Define Navigation Properties
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
using ECommerceAPI.Application.Models.Enums;

namespace ECommerceAPI.Application.Models;

public class ProductMedia
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string MediaUrl { get; set; }
    public MediaType Type { get; set; }  // Image or Video
    public int OrderIndex { get; set; }
    // Navigation Property
    public Product Product { get; set; }
}
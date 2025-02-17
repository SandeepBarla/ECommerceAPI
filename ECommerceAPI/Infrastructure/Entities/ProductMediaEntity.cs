using ECommerceAPI.Application.Models.Enums;
using ECommerceAPI.Infrastructure.Entities;

public class ProductMediaEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string MediaUrl { get; set; }
    public MediaType Type { get; set; }  // Image or Video
    public int OrderIndex { get; set; }
    
    // Navigation Property
    public ProductEntity Product { get; set; }
}
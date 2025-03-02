namespace ECommerceAPI.Infrastructure.Entities;

public class FavoriteEntity
{
    // ✅ Composite Primary Key: (UserId, ProductId)
    public int UserId { get; set; }
    public int ProductId { get; set; }

    // ✅ Navigation Properties
    public UserEntity User { get; set; } = null!;
    public ProductEntity Product { get; set; } = null!;
}
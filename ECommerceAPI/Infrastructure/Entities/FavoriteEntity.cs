namespace ECommerceAPI.Infrastructure.Entities;

public class FavoriteEntity
{
    public int Id { get; set; }

    // ✅ User who favorited the product
    public int UserId { get; set; }
    // public UserEntity User { get; set; } = null!;

    // ✅ Favorited Product
    public int ProductId { get; set; }
    // public ProductEntity Product { get; set; } = null!;
}
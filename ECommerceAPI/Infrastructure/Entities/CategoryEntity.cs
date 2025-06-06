namespace ECommerceAPI.Infrastructure.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Remove default "Lehenga"

    public List<ProductEntity> Products { get; set; } = new();
}
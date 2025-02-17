public class ProductListResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string PrimaryImageUrl { get; set; } // ✅ Only primary image
}
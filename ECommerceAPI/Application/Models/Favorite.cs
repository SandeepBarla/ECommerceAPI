namespace ECommerceAPI.Application.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
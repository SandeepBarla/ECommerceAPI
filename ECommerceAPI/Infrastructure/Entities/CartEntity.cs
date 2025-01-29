namespace ECommerceAPI.Infrastructure.Entities
{
    public class CartEntity
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key

        // Navigation Properties
        public UserEntity User { get; set; }
        public ICollection<CartItemEntity> CartItems { get; set; }
    }
}
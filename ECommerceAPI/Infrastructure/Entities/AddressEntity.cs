namespace ECommerceAPI.Infrastructure.Entities
{
  public class AddressEntity
  {
    public int Id { get; set; } // Primary Key
    public int UserId { get; set; } // Foreign Key
    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
    public string Phone { get; set; }
    public bool IsDefault { get; set; }

    // Navigation Properties
    public UserEntity User { get; set; }
  }
}
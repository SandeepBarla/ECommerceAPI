namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
  public class AddressResponse
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
    public string Phone { get; set; }
    public bool IsDefault { get; set; }
  }
}
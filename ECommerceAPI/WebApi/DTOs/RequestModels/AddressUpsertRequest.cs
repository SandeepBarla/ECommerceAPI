namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
  public class AddressUpsertRequest
  {
    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
    public string Phone { get; set; }
    public bool IsDefault { get; set; }
  }
}
namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    // Request sent from frontend to backend with the Google ID token
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }  // Token received from Google Sign-In
    }
}
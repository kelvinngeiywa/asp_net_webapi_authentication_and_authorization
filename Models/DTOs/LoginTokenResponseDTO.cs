namespace Asp_net_auth.Models.DTOs
{
    public class LoginTokenResponseDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}

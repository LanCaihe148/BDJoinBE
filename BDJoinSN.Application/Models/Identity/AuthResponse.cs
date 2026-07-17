namespace BDJoinSN.Application.Models.Identity
{
    public class AuthResponse
    {
        public string Id = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

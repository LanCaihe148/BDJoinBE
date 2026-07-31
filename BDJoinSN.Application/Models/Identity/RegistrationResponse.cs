namespace BDJoinSN.Application.Models.Identity
{
    public class RegistrationResponse
    {
        public string UserId = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}

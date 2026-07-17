namespace BDJoinSN.Application.Models.Identity
{
    public class RegistrationRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string RepeatPassword { get; set; } = string.Empty;
    }
}

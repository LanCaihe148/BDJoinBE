namespace BDJoinSN.Application.Models.Identity
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
        public bool InvalidateOtherSessions { get; set; } = true;
    }
}

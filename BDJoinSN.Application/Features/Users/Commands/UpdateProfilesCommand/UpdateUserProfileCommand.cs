

using MediatR;

namespace BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand
{
    public class UpdateUserProfileCommand : IRequest<bool>
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }

        public string? Username { get; set; }
        public string? Biography { get; set; }
        public string? Location { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? Birthday { get; set; }
    }
}

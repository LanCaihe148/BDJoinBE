using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BDJoinSN.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "49df5039-78fa-4378-9dbb-b65e7511edd5",
                    UserId = "3c0badec-c486-486a-84c2-a8873d1cbe77",
                },
                new IdentityUserRole<string>
                {
                    RoleId = "10cb8888-47b3-4d15-9336-a8c9248e8130",
                    UserId = "7d675971-3454-4ba1-b92c-c01109e37726",
                }
            );
        }
    }
}

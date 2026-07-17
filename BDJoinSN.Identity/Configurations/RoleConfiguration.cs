using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BDJoinSN.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                Id = "49df5039-78fa-4378-9dbb-b65e7511edd5",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Id = "10cb8888-47b3-4d15-9336-a8c9248e8130",
                    Name = "AuthUser",
                    NormalizedName = "AUTHUSER"
                });
        }
    }
}

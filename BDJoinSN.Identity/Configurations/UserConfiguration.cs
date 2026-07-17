using BDJoinSN.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BDJoinSN.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasData(
                new ApplicationUser
                {
                    Id = "3c0badec-c486-486a-84c2-a8873d1cbe77",
                    Name = "Efrain",
                    LastName = "Sandoval",
                    UserName = "e_sandoval32",
                    NormalizedUserName = "E_SANDOVAL32",
                    Email = "sandovalherest@gmail.com",
                    NormalizedEmail = "SANDOVALHEREST@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEOtA9rxxBOa75IvYaS7q/cm4BVErL6FOyeA2JL41Z5qP4Mfmzd4nYjgDeCA9PycKYA==",
                    SecurityStamp = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                    ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-f12345678901"
                },
                new ApplicationUser
                {
                    Id = "7d675971-3454-4ba1-b92c-c01109e37726",
                    Name = "Israel",
                    LastName = "Sandoval",
                    UserName = "israelfer1",
                    NormalizedUserName = "ISRAELFER1",
                    Email = "israelsandoval@gmail.com",
                    NormalizedEmail = "ISRAELSANDOVAL@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEO9QE7k9Hka6WNoRkibADq8NUXBCUv9F78UKXGd+RYjleD7ts3LesVW2RefKg1wb0w==",
                    SecurityStamp = "1cee0d76-7805-41b3-9b4d-c2b627456e92",
                    ConcurrencyStamp = "e4ee65a4-d2e6-4624-acfb-c33230b36681"
                });
        }
    }
}

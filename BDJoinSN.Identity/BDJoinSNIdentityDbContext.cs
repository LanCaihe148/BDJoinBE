using BDJoinSN.Identity.Configurations;
using BDJoinSN.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BDJoinSN.Identity
{
    public class BDJoinSNIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public BDJoinSNIdentityDbContext(DbContextOptions<BDJoinSNIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}

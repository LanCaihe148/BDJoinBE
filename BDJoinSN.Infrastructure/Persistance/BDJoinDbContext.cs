using BDJoinSN.Domain;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Persistance
{
    public class BDJoinDbContext : DbContext
    {
        public BDJoinDbContext(DbContextOptions<BDJoinDbContext> options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(up => up.Id);  
                entity.Property(up => up.Id).HasMaxLength(450);
                
            });

            builder.Entity<FriendRequest>(entity =>
            {
                entity.HasKey(fr => fr.Id);

                entity.HasOne(fr => fr.Sender)
                    .WithMany(up => up.SentFriendRequests)
                    .HasForeignKey(fr => fr.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fr => fr.Receiver)
                    .WithMany(up => up.ReceivedFriendRequests)
                    .HasForeignKey(fr => fr.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(fr => new { fr.SenderId, fr.ReceiverId }).IsUnique();
            });
        }
    }
}

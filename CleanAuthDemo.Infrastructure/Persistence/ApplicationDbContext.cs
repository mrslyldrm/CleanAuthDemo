using CleanAuthDemo.Infrastructure.Authentication;
using CleanAuthDemo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanAuthDemo.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RefreshTokenEntity>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

                entity.HasIndex(e => e.TokenHash)
                .IsUnique();

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.FamilyId);

                entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

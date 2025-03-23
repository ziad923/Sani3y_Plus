using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Models;
using System.Reflection.Emit;

namespace Sani3y_.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ContactUs> ContactMessages { get; set; }
        public DbSet<PreviousWork> PreviousWorks { get; set; }
        public DbSet<CraftsmanRecommendation> CraftsmanRecommendations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // NEED UPDATED
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "Adminn",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                     Id = "Userr",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = "Craftsmann",
                     Name = "Craftsman",
                     NormalizedName = "CRAFTSMAN"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
            builder.Entity<ContactUs>()
                    .Property(c => c.RequestNumber)
                    .HasMaxLength(50)  // Optional: set length or any other restrictions
                    .IsRequired();

            // Configure ServiceRequest Status as Enum
            builder.Entity<ServiceRequest>()
                .Property(s => s.Status)
                .HasConversion<string>();  // Store as string in DB

            builder.Entity<ServiceRequest>()
                .HasOne(s => s.Craftsman)
                .WithMany()
                .HasForeignKey(s => s.CraftsmanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceRequest>()
                 .HasOne(s => s.User)
                 .WithMany(u => u.Requests)
                 .HasForeignKey(s => s.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
             .HasOne(r => r.User) // User is the one who rated
             .WithMany(a => a.Ratings) // A user can have many ratings
             .HasForeignKey(r => r.UserId) // Foreign key is UserId
             .OnDelete(DeleteBehavior.Restrict); // Optional: Specify delete behavior

            // Configuring the relationship between Rating and Craftsman (who is rated)
            builder.Entity<Rating>()
                .HasOne(r => r.Craftsman) // Craftsman is being rated
                .WithMany() // A craftsman can have many ratings
                .HasForeignKey(r => r.CraftsmanId) // Foreign key is CraftsmanId
                .OnDelete(DeleteBehavior.Restrict); // Optional: Specify delete behavior;

            builder.Entity<ServiceRequest>()
                         .HasOne(sr => sr.Rating)
                       .WithOne(r => r.ServiceRequest)
                        .HasForeignKey<Rating>(r => r.ServiceRequestId)
                        .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CraftsmanRecommendation>()
           .HasOne(cr => cr.User)
           .WithMany(u => u.RecommendedCraftsmen)
           .HasForeignKey(cr => cr.UserId);
        }

    }
}

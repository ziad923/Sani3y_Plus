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
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<VerificationRequest> VerificationRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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

            builder.Entity<Profession>().HasData(
                    new Profession { Id = 1, Name = "كهربائي", ImagePath = "/professions/Lightning.png" },
                    new Profession { Id = 2, Name = "سبّــاك", ImagePath = "/professions/Plumbing.png" },
                    new Profession { Id = 3, Name = "نقاش", ImagePath = "/professions/Paint roller.png" },
                    new Profession { Id = 4, Name = "نجار موبيليا", ImagePath = "/professions/Saw.png" },
                    new Profession { Id = 5, Name = "نــجــار مـســلّح" , ImagePath = "/professions/Carpenter.png" },
                    new Profession { Id = 6, Name = "نجار باب وشباك", ImagePath = "/professions/Door.png" },
                    new Profession { Id = 7, Name = "حــــدّاد كـريـتال", ImagePath = "/professions/Mask.png" },
                    new Profession { Id = 8, Name = "حــــدّاد مـســلّح", ImagePath = "/professions/Steel.png" },
                    new Profession { Id = 9, Name = "بــنّــــاء", ImagePath = "/professions/Brickwall.png" },
                    new Profession { Id = 10, Name = "مُــبيّـض مــحــارة", ImagePath = "/professions/Brick wall.png" },
                    new Profession { Id = 11, Name = "مُــبلّــط ســيرامــيك", ImagePath = "/professions/Ceramics.png" },
                    new Profession { Id = 12, Name = "مُــبلّــط رخـــــام", ImagePath = "/professions/Marble.png" },
                    new Profession { Id = 13, Name = "إنــترلـــوك", ImagePath = "/professions/Tiles.png" },
                    new Profession { Id = 14, Name = "حجر فرعوني", ImagePath = "/professions/Stone.png" },
                    new Profession { Id = 15, Name = "ونش بناء", ImagePath = "/professions/Steel-1.png" },
                    new Profession { Id = 16, Name = "فني تركيبات", ImagePath = "/professions/Wrench.png" },
                    new Profession { Id = 17, Name = "لـــحّـــام", ImagePath = "/professions/Welding.png" },
                    new Profession { Id = 18, Name = "رافــعـة أثـــاث", ImagePath = "/professions/Crane truck.png" },
                    new Profession { Id = 19, Name = "ألــومــيتــال", ImagePath = "/professions/Window.png" },
                    new Profession { Id = 20, Name = "جبسن بورد", ImagePath = "/professions/Drywall.png" }
    );
            

            var adminUser = new AppUser
            {
                Id = "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33",
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@sanai3yplus.com",
                NormalizedEmail = "ADMIN@SANAI3YPLUS.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEJfhTswhLI5PHIqSW0TEObECD0w5YUBpybAois77tfo+ANCAiCxsXoCymyuOopZ6fA==",
                SecurityStamp = "RKVK74DEVATHOYBU7MWSMQM2I6R2EAJE",
                ConcurrencyStamp = "98212b0c-77d1-4bb4-9293-e307ff6718d6",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,

                
                FirstName = "Admin",
                LastName = "User",
                Governorate = "",   
                Location = "",       
                Role = "Admin",
                ProfileImagePath = null,
                CardImagePath = null,
                ProfessionId = null,
                IsTrusted = null,
                GoogleId = null,
                RefreshToken = "zQg3XUt1Rf0pdQP0TIW5XvySUIxmb339ZDOx0Q69OiI=",
                RefreshTokenExpiryTime = new DateTime(2025, 5, 22, 9, 56, 56, 993, DateTimeKind.Utc).AddTicks(8356)
            };

            builder.Entity<AppUser>().HasData(adminUser);

            // Assign Admin Role to Admin User
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "Adminn",
                    UserId = "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33"
                }
            );

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

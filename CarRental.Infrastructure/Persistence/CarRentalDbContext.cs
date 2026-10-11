using CarRental.Domain.Entities;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Persistence
{
    public class CarRentalDbContext : IdentityDbContext<ApplicationUser>
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options)
        {

        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FavoriteList> FavoriteLists { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(
    ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CarRental.Domain.Entities.Car>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Brand)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Model)
                            .IsRequired()
                            .HasMaxLength(100);

                entity.Property(c => c.Color)
                            .IsRequired()
                            .HasMaxLength(50);

                entity.Property(c => c.Transmission)
                            .IsRequired()
                            .HasMaxLength(50);

                entity.Property(c => c.FuelType)
                            .IsRequired()
                            .HasMaxLength(50);

                entity.Property(c => c.PricePerDay)
                            .HasPrecision(18, 2);

                builder.Entity<Booking>()
    .HasOne<ApplicationUser>()
    .WithMany()
    .HasForeignKey(b => b.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);


                builder.Entity<Booking>()
    .HasOne(b => b.Car)
    .WithMany()
    .HasForeignKey(b => b.CarId)
    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Booking>()
                    .HasOne(b => b.PickupBranch)
                    .WithMany()
                    .HasForeignKey(b => b.PickupBranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Booking>()
                    .HasOne(b => b.ReturnBranch)
                    .WithMany()
                    .HasForeignKey(b => b.ReturnBranchId)
                    .OnDelete(DeleteBehavior.Restrict);


                builder.Entity<Booking>()
    .Property(b => b.TotalPrice)
    .HasPrecision(18, 2);

                builder.Entity<Booking>()
                    .Property(b => b.Discount)
                    .HasPrecision(18, 2);

                builder.Entity<Payment>(entity =>
                {
                    entity.HasKey(p => p.Id);

                    entity.Property(p => p.Amount)
                        .HasPrecision(18, 2);

                    entity.Property(p => p.TransactionId)
                        .HasMaxLength(200);

                    entity.Property(p => p.StripeSessionId)
                        .HasMaxLength(200);

                    entity.Property(p => p.StripeCheckoutUrl)
                       .HasMaxLength(1000);

                    entity.HasOne(p => p.Booking)
                        .WithOne(b => b.Payment)
                        .HasForeignKey<Payment>(p => p.BookingId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                builder.Entity<Conversation>()
    .HasMany(c => c.Messages)
    .WithOne(m => m.Conversation)
    .HasForeignKey(m => m.ConversationId)
    .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<Message>()
                    .Property(m => m.Content)
                    .HasMaxLength(2000);

                builder.Entity<Conversation>()
                    .Property(c => c.User1Id)
                    .IsRequired();

                builder.Entity<Conversation>()
                    .Property(c => c.User2Id)
                    .IsRequired();

                builder.Entity<FavoriteList>()
.HasOne<ApplicationUser>()
.WithMany()
.HasForeignKey(b => b.UserId)
.OnDelete(DeleteBehavior.Restrict);


                builder.Entity<FavoriteList>()
    .HasOne(b => b.Car)
    .WithMany()
    .HasForeignKey(b => b.CarId)
    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Review>()
    .HasIndex(r => new { r.UserId, r.BookingId })
    .IsUnique();

                builder.Entity<Review>()
.HasOne<ApplicationUser>()
.WithMany()
.HasForeignKey(b => b.UserId)
.OnDelete(DeleteBehavior.Restrict);


                builder.Entity<Review>()
    .HasOne(b => b.Car)
    .WithMany()
    .HasForeignKey(b => b.CarId)
    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Review>()
                    .HasOne(r => r.Booking)
                    .WithMany()
                    .HasForeignKey(r => r.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

        }
    }
}

using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Data
{
    public class CakerDbContext(DbContextOptions<CakerDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Confectioner> Confectioners { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();

            modelBuilder
                .Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Confectioner>()
                .HasOne(c => c.User)
                .WithOne(u => u.Confectioner)
                .HasForeignKey<Confectioner>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder
                .Entity<Cake>()
                .HasOne(c => c.Confectioner)
                .WithMany(c => c.Cakes)
                .HasForeignKey(c => c.ConfectionerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Cake)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CakeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

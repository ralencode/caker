using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Data
{
    public class CakerDbContext(DbContextOptions<CakerDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Confectioner> Confectioners { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

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
                .Entity<Message>()
                .HasOne(m => m.From)
                .WithMany(u => u.MessagesFrom)
                .HasForeignKey(m => m.FromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Message>()
                .HasOne(m => m.To)
                .WithMany(u => u.MessagesTo)
                .HasForeignKey(m => m.ToId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder
                .Entity<Feedback>()
                .HasOne(f => f.Confectioner)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.ConfectionerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Feedback>()
                .HasOne(f => f.Cake)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CakeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

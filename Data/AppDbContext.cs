using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;

namespace ECommerceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Ensure that each user has only one cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
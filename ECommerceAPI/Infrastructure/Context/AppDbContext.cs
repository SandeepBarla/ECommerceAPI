using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define DbSets for each entity
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductMediaEntity> ProductMedia { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderProductEntity> OrderProducts { get; set; }
        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure each user has only one cart
            modelBuilder.Entity<CartEntity>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<CartEntity>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationship between Cart and CartItems
            modelBuilder.Entity<CartEntity>()
                .HasMany(c => c.CartItems)
                .WithOne()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define Composite Primary Key for OrderProduct
            modelBuilder.Entity<OrderProductEntity>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            // Ensure relationship between Order and OrderProducts
            modelBuilder.Entity<OrderEntity>()
                .HasMany(o => o.OrderProducts)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Media)
                .WithOne(m => m.Product)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}

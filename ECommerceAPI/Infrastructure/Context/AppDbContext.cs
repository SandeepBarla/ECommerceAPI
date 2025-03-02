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

        // ✅ DbSet for Categories
        public DbSet<CategoryEntity> Categories { get; set; }

        // ✅ DbSet for Sizes
        public DbSet<SizeEntity> Sizes { get; set; }

        // ✅ DbSet for Favorites
        public DbSet<FavoriteEntity> Favorites { get; set; }

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
            
            // // ✅ Many-to-Many Configuration for Favorites
            // modelBuilder.Entity<FavoriteEntity>()
            //     .HasKey(f => new { f.UserId, f.ProductId });
            //
            // modelBuilder.Entity<FavoriteEntity>()
            //     .HasOne(f => f.Product)
            //     .WithMany(p => p.Favorites)
            //     .HasForeignKey(f => f.ProductId);
            //
            // modelBuilder.Entity<FavoriteEntity>()
            //     .HasOne(f => f.User)
            //     .WithMany()
            //     .HasForeignKey(f => f.UserId);
            //
            // ✅ One-to-Many Relationship for Category
            // modelBuilder.Entity<ProductEntity>()
            //     .HasOne(p => p.Category)
            //     .WithMany(c => c.Products)
            //     .HasForeignKey(p => p.CategoryId);
            
            // ✅ One-to-Many Relationship for Size
            // modelBuilder.Entity<ProductEntity>()
            //     .HasOne(p => p.Size)
            //     .WithMany(s => s.Products)
            //     .HasForeignKey(p => p.SizeId);
            
        }
    }
}

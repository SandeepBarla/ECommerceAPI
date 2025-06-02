using ECommerceAPI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Context;

public static class DataSeeder
{
  public static async Task SeedDataAsync(AppDbContext context)
  {
    // Ensure categories exist
    if (!await context.Categories.AnyAsync())
    {
      var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = 1, Name = "Lehenga" },
                new CategoryEntity { Id = 6, Name = "Saree" }
            };

      await context.Categories.AddRangeAsync(categories);
      await context.SaveChangesAsync();

      // Update the sequence to avoid conflicts (only for PostgreSQL)
      if (context.Database.IsNpgsql())
      {
        await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Categories\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Categories\"));");
      }
    }
    else
    {
      // Check if Saree category exists, if not add it
      var sareeCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Saree");
      if (sareeCategory == null)
      {
        // Find the highest existing ID and use the next one
        var maxId = await context.Categories.MaxAsync(c => (int?)c.Id) ?? 0;
        var newSareeCategory = new CategoryEntity { Id = maxId + 1, Name = "Saree" };
        await context.Categories.AddAsync(newSareeCategory);
        await context.SaveChangesAsync();

        // Update the sequence to avoid conflicts (only for PostgreSQL)
        if (context.Database.IsNpgsql())
        {
          await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Categories\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Categories\"));");
        }
      }
    }

    // Ensure sizes exist
    if (!await context.Sizes.AnyAsync())
    {
      var sizes = new List<SizeEntity>
            {
                new SizeEntity { Id = 1, Name = "S" },
                new SizeEntity { Id = 2, Name = "M" },
                new SizeEntity { Id = 3, Name = "L" },
                new SizeEntity { Id = 4, Name = "XL" },
                new SizeEntity { Id = 5, Name = "XXL" }
            };

      await context.Sizes.AddRangeAsync(sizes);
      await context.SaveChangesAsync();

      // Update the sequence to avoid conflicts (only for PostgreSQL)
      if (context.Database.IsNpgsql())
      {
        await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Sizes\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Sizes\"));");
      }
    }
  }
}
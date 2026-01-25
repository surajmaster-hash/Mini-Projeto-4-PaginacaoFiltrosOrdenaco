using Catalog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(CatalogDbContext dbContext)
    {
        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Almond Milk", Description = "Unsweetened almond milk.", Price = 3.49m, CreatedAt = now.AddDays(-12) },
            new() { Id = Guid.NewGuid(), Name = "Coffee Beans", Description = "Single origin medium roast.", Price = 12.99m, CreatedAt = now.AddDays(-25) },
            new() { Id = Guid.NewGuid(), Name = "Matcha Powder", Description = "Ceremonial grade matcha.", Price = 19.50m, CreatedAt = now.AddDays(-7) },
            new() { Id = Guid.NewGuid(), Name = "Olive Oil", Description = "Extra virgin olive oil.", Price = 14.75m, CreatedAt = now.AddDays(-30) },
            new() { Id = Guid.NewGuid(), Name = "Dark Chocolate", Description = "70% cacao dark chocolate.", Price = 4.25m, CreatedAt = now.AddDays(-3) },
            new() { Id = Guid.NewGuid(), Name = "Sea Salt", Description = "Flaky sea salt.", Price = 2.10m, CreatedAt = now.AddDays(-20) },
            new() { Id = Guid.NewGuid(), Name = "Pasta", Description = "Durum wheat fusilli.", Price = 3.20m, CreatedAt = now.AddDays(-9) },
            new() { Id = Guid.NewGuid(), Name = "Tomato Sauce", Description = "Slow cooked tomato sauce.", Price = 5.60m, CreatedAt = now.AddDays(-15) },
            new() { Id = Guid.NewGuid(), Name = "Green Tea", Description = "Loose leaf green tea.", Price = 8.30m, CreatedAt = now.AddDays(-18) },
            new() { Id = Guid.NewGuid(), Name = "Granola", Description = "Honey almond granola.", Price = 6.75m, CreatedAt = now.AddDays(-6) },
            new() { Id = Guid.NewGuid(), Name = "Coconut Water", Description = "Natural coconut water.", Price = 3.95m, CreatedAt = now.AddDays(-11) },
            new() { Id = Guid.NewGuid(), Name = "Yogurt", Description = "Greek yogurt plain.", Price = 4.80m, CreatedAt = now.AddDays(-4) },
            new() { Id = Guid.NewGuid(), Name = "Peanut Butter", Description = "Creamy peanut butter.", Price = 5.10m, CreatedAt = now.AddDays(-22) },
            new() { Id = Guid.NewGuid(), Name = "Honey", Description = "Wildflower honey.", Price = 7.40m, CreatedAt = now.AddDays(-27) },
            new() { Id = Guid.NewGuid(), Name = "Basmati Rice", Description = "Aromatic basmati rice.", Price = 9.99m, CreatedAt = now.AddDays(-14) }
        };

        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();
    }
}

using System.Text.Json;
using LegacyOrderService.Entities;
using LegacyOrderService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LegacyOrderService.Services
{
    public interface IDataSeeder
    {
        Task SeedProductsAsync(OrderContext context, CancellationToken cancellationToken = default);
    }

    public class DataSeeder(IConfiguration configuration) : IDataSeeder
    {
        private readonly string _productsFilePath = configuration["DataSeeding:ProductsFilePath"] ?? "products.json";

        public async Task SeedProductsAsync(OrderContext context, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_productsFilePath))
            {
                Console.WriteLine($"Products file not found: {_productsFilePath}");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_productsFilePath, cancellationToken);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var newProducts = JsonSerializer.Deserialize<List<Product>>(json, options);

                if (newProducts == null || newProducts.Count == 0)
                {
                    Console.WriteLine("No products in JSON file");
                    return;
                }

                var existingProducts = await context.Products.ToListAsync(cancellationToken);

                bool productsChanged = existingProducts.Count != newProducts.Count ||
                    !existingProducts.SequenceEqual(newProducts, new ProductEqualityComparer());

                if (productsChanged)
                {
                    Console.WriteLine("Products changed - reloading...");
                    await context.Orders.ExecuteDeleteAsync(cancellationToken);

                    await context.Products.ExecuteDeleteAsync(cancellationToken);

                    await context.Products.AddRangeAsync(newProducts, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                    Console.WriteLine("Products updated from JSON file.");
                }
                else
                {
                    Console.WriteLine("Products unchanged");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products from {_productsFilePath}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private class ProductEqualityComparer : IEqualityComparer<Product>
        {
            public bool Equals(Product? x, Product? y)
            {
                if (x == null || y == null) return x == y;
                return x.Id == y.Id && x.Name == y.Name && x.Price == y.Price;
            }

            public int GetHashCode(Product obj)
            {
                return HashCode.Combine(obj.Id, obj.Name, obj.Price);
            }
        }
    }
}

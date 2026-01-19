// Data/ProductRepository.cs
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderContext _context;

        public ProductRepository(OrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<double> GetPriceAsync(string productName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("productName required", nameof(productName));

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == productName, cancellationToken);

            if (product == null)
                throw new InvalidOperationException("Product not found");

            return product.Price;
        }

        public async Task<(int ProductId, double Price)> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("productName required", nameof(productName));

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == productName, cancellationToken);

            if (product == null)
                throw new InvalidOperationException("Product not found");

            return (product.Id, product.Price);
        }
    }
}

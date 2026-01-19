using System.Threading;
using System.Threading.Tasks;

namespace LegacyOrderService.Data
{
    public interface IProductRepository
    {
        Task<double> GetPriceAsync(string productName, CancellationToken cancellationToken = default);
        Task<(int ProductId, double Price)> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default);
    }
}

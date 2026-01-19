using LegacyOrderService.Entities;

namespace LegacyOrderService.Data
{
    public interface IOrderRepository
    {
        Task SaveAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}

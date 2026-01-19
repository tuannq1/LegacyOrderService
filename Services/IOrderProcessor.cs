namespace LegacyOrderService.Data
{
    public interface IOrderProcessor
    {
        Task ProcessOrderAsync(CancellationToken cancellationToken = default);
    }
}

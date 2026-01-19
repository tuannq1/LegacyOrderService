using LegacyOrderService.Data;

namespace LegacyOrderService
{
    public class App
    {
        private readonly IOrderProcessor _processor;

        public App(IOrderProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _processor.ProcessOrderAsync(cancellationToken);
                Console.WriteLine("Done.");
                return 0;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation cancelled by user.");
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using LegacyOrderService.Entities;
using LegacyOrderService.Data;

namespace LegacyOrderService.Data
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderProcessor(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task ProcessOrderAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Enter customer name:");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name is required");
            name = ValidateName(name);

            Console.WriteLine("Enter product name:");
            var productName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required");

            // Get product by name to obtain ProductId and Price
            var (productId, price) = await _productRepository.GetProductByNameAsync(productName!, cancellationToken);

            Console.WriteLine("Enter quantity:");
            var qtyStr = Console.ReadLine();
            if (!int.TryParse(qtyStr, out var qty) || qty <= 0)
                throw new ArgumentException("Quantity must be a positive integer");

            Console.WriteLine("Processing order...");

            Order order = new Order
            {
                CustomerName = name!,
                ProductId = productId,
                Quantity = qty,
                Price = price
            };

            Console.WriteLine("Saving order to database...");
            await _orderRepository.SaveAsync(order, cancellationToken);

            // Query the saved order from database and display it
            var savedOrder = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
            if (savedOrder != null)
            {
                double total = savedOrder.Quantity * savedOrder.Price;

                Console.WriteLine("Order complete!");
                Console.WriteLine($"Order ID: {savedOrder.Id}");
                Console.WriteLine($"Customer: {savedOrder.CustomerName}");
                Console.WriteLine($"ProductId: {savedOrder.ProductId}");
                Console.WriteLine($"Quantity: {savedOrder.Quantity}");
                Console.WriteLine($"Total: ${total:F2}");
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve saved order from database");
            }
        }

        private static string ValidateName(string name)
        {
            if (name.Length < 2 || name.Length > 100)
                throw new ArgumentException("Customer name must be between 2 and 100 characters.");

            return name;
        }
    }
}

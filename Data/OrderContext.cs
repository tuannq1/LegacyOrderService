using LegacyOrderService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LegacyOrderService.Data;
using LegacyOrderService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Prefer configuration, fall back to default file-based DB
                    string conn = context.Configuration["ConnectionStrings:Default"] ?? "Data Source=orders.db";
                    const string dsPrefix = "Data Source=";
                    if (conn.StartsWith(dsPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var ds = conn.Substring(dsPrefix.Length).Trim();
                        if (!Path.IsPathRooted(ds))
                            ds = Path.Combine(AppContext.BaseDirectory, ds);
                        conn = $"{dsPrefix}{ds}";
                    }

                    services.AddDbContext<OrderContext>(opt => opt.UseSqlite(conn));

                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();
                    services.AddScoped<IOrderProcessor, OrderProcessor>();
                    services.AddScoped<IDataSeeder, DataSeeder>();
                    services.AddScoped<App>();
                })
                .ConfigureLogging(logging => logging.ClearProviders().AddConsole())
                .Build();

            Console.WriteLine("Welcome to Order Processor!");

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Cancellation requested...");
                cts.Cancel();
            };

            await host.StartAsync(cts.Token);

            using (var scope = host.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<OrderContext>();
                await ctx.Database.MigrateAsync(cts.Token);

                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                await seeder.SeedProductsAsync(ctx, cts.Token);
            }

            var app = host.Services.GetRequiredService<App>();
            int result = await app.RunAsync(cts.Token);

            await host.StopAsync(cts.Token);
            return result;
        }
    }
}

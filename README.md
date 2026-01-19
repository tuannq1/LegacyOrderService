# LegacyOrderService

A .NET 10 console app for processing orders. Started as legacy code and was modernized with async/await, EF Core, dependency injection, and proper database design.

## Quick Start

```bash
dotnet build
dotnet run
```

Press Ctrl+C to exit.

## What It Does

1. Takes customer name and product name
2. Looks up product from database
3. Calculates order total
4. Saves order to SQLite

## Features

- **Async throughout** - All I/O uses async/await
- **EF Core migrations** - Schema version controlled
- **Transaction safety** - Rollback on error
- **Configurable products** - Load from `products.json`
- **Non-blocking input** - Ctrl+C works immediately

## Project Structure

```
Entities/        # Order, Product entities
Data/            # EF Core context, repositories
Services/        # OrderProcessor, DataSeeder
Utilities/       # AsyncConsole helper
Migrations/      # Database migrations
```

## Configuration

- `appsettings.json` - DB connection, seeding config
- `products.json` - Product catalog

## How Products Work

On startup:
- Loads products from `products.json`
- If products changed, deletes all orders and reloads
- If unchanged, keeps orders

Edit `products.json` and restart to change products.

## Database

Tables:
- **Products** - Id, Name (unique index), Price
- **Orders** - Id, CustomerName, ProductId (FK), Quantity, Price

Migrations run automatically on startup.

## Improvements Made

- ✅ SQL injection fixed (parameterized queries)
- ✅ Transactions with rollback
- ✅ Full async/await with CancellationToken
- ✅ Dependency injection setup
- ✅ EF Core with migrations
- ✅ Input validation
- ✅ Proper error handling
- ✅ Resource cleanup

## Example Run

```
Welcome to Order Processor!
Products unchanged
Enter customer name: Alice
Enter product name: Widget
Enter quantity: 2
Processing order...
Order complete!
Customer: Alice
ProductId: 1
Quantity: 2
Total: $25.98
Saving order to database...
Done.
```

using Domain.Products;

namespace Domain;

public static class ProductSeeder
{
    public static void SeedProducts(ApplicationDbContext context)
        {
            if (context.Products.Any())
            {
                return;
            }

            var products = new List<Product>
            {
                // Electronics
                new Product { ProductName = "Wireless Headphones", Description = "Premium noise-cancelling headphones", Price = 299.99m, StockQuantity = 50, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Smart Watch", Description = "Fitness tracking smartwatch", Price = 399.99m, StockQuantity = 30, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Laptop Stand", Description = "Ergonomic aluminum laptop stand", Price = 49.99m, StockQuantity = 100, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "USB-C Hub", Description = "7-in-1 USB-C multiport adapter", Price = 79.99m, StockQuantity = 75, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 59.99m, StockQuantity = 120, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Mechanical Keyboard", Description = "RGB mechanical gaming keyboard", Price = 149.99m, StockQuantity = 40, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Webcam HD", Description = "1080p HD webcam with microphone", Price = 89.99m, StockQuantity = 60, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Portable SSD 1TB", Description = "Fast external solid state drive", Price = 129.99m, StockQuantity = 55, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Clothing
                new Product { ProductName = "Cotton T-Shirt", Description = "Comfortable cotton t-shirt", Price = 24.99m, StockQuantity = 200, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Denim Jeans", Description = "Classic blue denim jeans", Price = 79.99m, StockQuantity = 150, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Hoodie", Description = "Warm fleece hoodie", Price = 59.99m, StockQuantity = 100, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Running Shoes", Description = "Lightweight running shoes", Price = 119.99m, StockQuantity = 80, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Winter Jacket", Description = "Insulated winter jacket", Price = 199.99m, StockQuantity = 45, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Baseball Cap", Description = "Adjustable baseball cap", Price = 29.99m, StockQuantity = 150, Category = "Clothing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Home & Kitchen
                new Product { ProductName = "Coffee Maker", Description = "12-cup programmable coffee maker", Price = 89.99m, StockQuantity = 35, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Blender", Description = "High-speed blender for smoothies", Price = 69.99m, StockQuantity = 50, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Cooking Pan Set", Description = "Non-stick cooking pan set", Price = 129.99m, StockQuantity = 40, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Knife Set", Description = "Professional 8-piece knife set", Price = 99.99m, StockQuantity = 60, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Vacuum Cleaner", Description = "Cordless stick vacuum", Price = 249.99m, StockQuantity = 25, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Air Fryer", Description = "Digital air fryer 5.8 quart", Price = 119.99m, StockQuantity = 45, Category = "Home & Kitchen", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Books
                new Product { ProductName = "Programming Guide", Description = "Complete guide to modern programming", Price = 49.99m, StockQuantity = 100, Category = "Books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Mystery Novel", Description = "Bestselling mystery thriller", Price = 19.99m, StockQuantity = 200, Category = "Books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Cookbook", Description = "Healthy recipes cookbook", Price = 34.99m, StockQuantity = 85, Category = "Books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Sports & Outdoors
                new Product { ProductName = "Yoga Mat", Description = "Non-slip yoga mat with carry strap", Price = 39.99m, StockQuantity = 120, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Dumbbell Set", Description = "Adjustable dumbbell set 50lbs", Price = 199.99m, StockQuantity = 30, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Camping Tent", Description = "4-person waterproof tent", Price = 159.99m, StockQuantity = 20, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Water Bottle", Description = "Insulated stainless steel bottle", Price = 29.99m, StockQuantity = 150, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Backpack", Description = "Hiking backpack 40L capacity", Price = 89.99m, StockQuantity = 65, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Resistance Bands", Description = "Set of 5 resistance bands", Price = 24.99m, StockQuantity = 180, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { ProductName = "Jump Rope", Description = "Speed jump rope for fitness", Price = 14.99m, StockQuantity = 200, Category = "Sports & Outdoors", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
}
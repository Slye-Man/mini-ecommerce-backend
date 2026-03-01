using Domain.CartItems;
using Microsoft.EntityFrameworkCore;
using Domain.Orders;
using Domain.Users;
using Domain.Products;
using Domain.Carts;
using Domain.OrderItems;


namespace Domain;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Session> Sessions { get; set; }
}
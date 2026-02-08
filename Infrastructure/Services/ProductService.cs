using Domain;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<Product>> GetAllProducts(
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            string? search = null)
        {
            var query = _context.Products.AsQueryable();
            
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }
            
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => 
                    p.ProductName.Contains(search) || 
                    (p.Description != null && p.Description.Contains(search)));
            }
            
            query = query.Where(p => p.StockQuantity > 0);
            
            query = query.OrderBy(p => p.ProductName);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductById(int productId)
        {
            try
            {
                return await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product by ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<List<Product>> GetProductsByCategory(string category)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.Category == category && p.StockQuantity > 0)
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products by category: {Category}", category);
                throw;
            }
        }

        public async Task<List<string?>> GetAllCategories()
        {

            return await _context.Products
                            .Where(p => p.StockQuantity > 0)
                            .Select(p => p.Category)
                            .Distinct()
                            .OrderBy(c => c)
                            .ToListAsync();
                        
        }
}
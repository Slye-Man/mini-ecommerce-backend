using Domain.Products;

namespace Infrastructure.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProducts(string? category = null, decimal? minPrice = null, decimal? maxPrice = null, string? search = null);
    Task<Product?> GetProductById(int productId);
    Task<List<Product>> GetProductsByCategory(string category);
    Task<List<string>> GetAllCategories();
}
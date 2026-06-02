using Microsoft.EntityFrameworkCore;
using System.Linq;

using YAGOT_2._0.Models;

namespace YAGOT_2._0.Services;

public class ProductService
{
    private readonly NeondbContext _context;

    public ProductService(NeondbContext context)
    {
        _context = context;
    }

    private static string SanitizeLikeTerm(string raw) =>
        raw.Replace("%", string.Empty).Replace("_", string.Empty);

    public Task<int> NextCounter()
    {
        int nextId = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;
        return Task.FromResult(nextId);
    }


    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return Task.FromResult(_context.Products.AsEnumerable());
    }

    public Task<Product?> GetProductByIdAsync(int id)
    {
        return Task.FromResult(_context.Products.FirstOrDefault(p => p.Id == id));
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return Task.FromResult(_context.Categories.AsEnumerable());
    }

    /// <summary>بحث في الاسم والوصف واسم التصنيف مع AsNoTracking.</summary>
    public async Task<List<Product>> SearchAsync(string? query, int? categoryId, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> q = _context.Products
            .AsNoTracking()
            .Include(p => p.Category);

        if (categoryId is > 0)
            q = q.Where(p => p.Categoryid == categoryId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = SanitizeLikeTerm(query.Trim());
            if (term.Length > 0)
            {
                q = q.Where(p =>
                    EF.Functions.ILike(p.Name, $"%{term}%") ||
                    (p.Description != null && EF.Functions.ILike(p.Description, $"%{term}%")) ||
                    EF.Functions.ILike(p.Category.Name, $"%{term}%"));
            }
        }

        return await q.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<List<SearchSuggestionDto>> SearchSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        var term = SanitizeLikeTerm(query);
        if (term.Length < 2)
            return new List<SearchSuggestionDto>();

        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p =>
                EF.Functions.ILike(p.Name, $"%{term}%") ||
                (p.Description != null && EF.Functions.ILike(p.Description, $"%{term}%")) ||
                EF.Functions.ILike(p.Category.Name, $"%{term}%"))
            .OrderBy(p => p.Name)
            .Take(8)
            .Select(p => new SearchSuggestionDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.Imageurl
            })
            .ToListAsync(cancellationToken);
    }

    //public Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    //{
    //    return Task.FromResult(FakeDb.Products.Where(p => p.CategoryId == categoryId));
    //}

    //public Task<IEnumerable<Product>> SearchProductsAsync(string query)
    //{
    //    if (string.IsNullOrWhiteSpace(query)) return GetAllProductsAsync();

    //    var results = FakeDb.Products.Where(p => 
    //        p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || 
    //        p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

    //    return Task.FromResult(results);
    //}

    //public Task AddProductAsync(Product product)
    //{
    //    product.Id = FakeDb.NextProductId();
    //    product.CreatedAt = DateTime.UtcNow;
    //    FakeDb.Products.Add(product);
    //    return Task.CompletedTask;
    //}

    //public Task UpdateProductAsync(Product product)
    //{
    //    var existing = FakeDb.Products.FirstOrDefault(p => p.Id == product.Id);
    //    if (existing != null)
    //    {
    //        existing.Name = product.Name;
    //        existing.Description = product.Description;
    //        existing.Price = product.Price;
    //        existing.StockQuantity = product.StockQuantity;
    //        existing.CategoryId = product.CategoryId;
    //        if (!string.IsNullOrEmpty(product.ImageUrl))
    //        {
    //            existing.ImageUrl = product.ImageUrl;
    //        }
    //    }
    //    return Task.CompletedTask;
    //}

    //public Task DeleteProductAsync(int id)
    //{
    //    var product = FakeDb.Products.FirstOrDefault(p => p.Id == id);
    //    if (product != null) FakeDb.Products.Remove(product);
    //    return Task.CompletedTask;
    //}
}

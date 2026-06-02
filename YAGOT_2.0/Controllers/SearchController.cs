using Microsoft.AspNetCore.Mvc;
using YAGOT_2._0.Models;
using YAGOT_2._0.Services;

namespace YAGOT_2._0.Controllers;

public class SearchController : Controller
{
    private readonly ProductService _productService;

    public SearchController(ProductService productService)
    {
        _productService = productService;
    }

    /// <summary>صفحة نتائج البحث: /Search?q=...&amp;category=...</summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? q, int? category, CancellationToken cancellationToken)
    {
        var categories = (await _productService.GetCategoriesAsync()).OrderBy(c => c.Name).ToList();
        ViewBag.Categories = categories;
        ViewBag.Query = q;
        ViewBag.CategoryId = category;

        var hasText = !string.IsNullOrWhiteSpace(q);
        var hasCategory = category is > 0;
        if (!hasText && !hasCategory)
        {
            return View(Enumerable.Empty<Product>());
        }

        var products = await _productService.SearchAsync(
            hasText ? q!.Trim() : null,
            hasCategory ? category : null,
            cancellationToken);
        return View(products);
    }

    [HttpGet("/api/search/suggestions")]
    public async Task<IActionResult> Suggestions([FromQuery] string? q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            return Json(Array.Empty<SearchSuggestionDto>());

        var items = await _productService.SearchSuggestionsAsync(q.Trim(), cancellationToken);
        return Json(items);
    }
}

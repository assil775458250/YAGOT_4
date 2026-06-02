using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using YAGOT_2._0.Models;
using YAGOT_2._0.Services;

namespace YAGOT_2._0.Controllers;

public class ProductsController : Controller
{
    private readonly NeondbContext _context;

    public ProductsController(NeondbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {
        var productsFromDb = await _context.Products.Include(p => p.Category).Where(p => p.Stockquantity > 0).ToListAsync();
        var categoriesFromDb = await _context.Categories.ToListAsync();
        var model = new ViewModels
        {
            Products = productsFromDb,
            Categories = categoriesFromDb
        };
        var categoryList =  model.Categories.ToList();
        ViewBag.Categories = categoryList.ToList();

        if (categoryId.HasValue)
        {
            var category = categoryList.FirstOrDefault(c => c.Id == categoryId.Value);
            ViewBag.CategoryName = category?.Name;
            ViewBag.CategoryId = categoryId;

            List<Product> allProducts = model.Products.Where(p => p.Categoryid == categoryId).ToList();
            if (categoryId == -100)
            {
                allProducts = model.Products.ToList();

            }
            return View(allProducts.ToList());
        }


        List<Product>  allProducts_all = model.Products.ToList();
        
       
        ViewBag.CategoryId = null;
        return View(allProducts_all);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }
    public async Task<IActionResult> trash(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }
}

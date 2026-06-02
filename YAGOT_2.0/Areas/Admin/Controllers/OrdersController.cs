using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YAGOT_2._0.Models;

namespace YAGOT_2._0.Areas.Admin.Controllers;

[Area("Admin")]
public class OrdersController : Controller
{
    private readonly NeondbContext _context;

    public OrdersController(NeondbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? status)
    {
        var filter = OrderStatusFilters.NormalizeQuery(status);

        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Orderitems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.Orderdate)
            .AsQueryable();

        if (filter != null)
            query = query.Where(o => o.Status == filter);

        var orders = await query.ToListAsync();
        ViewBag.StatusFilter = string.IsNullOrWhiteSpace(status) ? OrderStatusFilters.All : status;
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? returnStatus)
    {
        var now = DateTime.Now;
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order != null)
        {
            order.Status = status;
            order.TimeState = now;
        }

        await _context.SaveChangesAsync();

        var redirectStatus = string.IsNullOrWhiteSpace(returnStatus)
            ? OrderStatusFilters.All
            : returnStatus;
        return RedirectToAction(nameof(Index), new { status = redirectStatus });
    }
}

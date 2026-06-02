using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YAGOT_2._0.Services;
using YAGOT_2._0.Models;

namespace YAGOT_2._0.Controllers;

public class OrdersController : Controller
{
    private readonly OrderService _orderService;
    private  int _mockUserId = 1;
    private readonly NeondbContext _context;

    public OrdersController(OrderService orderService, NeondbContext context)
    {
        _context = context;
        _orderService = orderService;
        
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = _context.Users.FirstOrDefault(n => n.Name == User.Identity.Name);
            if (user != null)
            {
                _mockUserId = user.Id;
            }
        }
        else
        {

            return RedirectToAction("Auth", "Account", "/Orders/Index");
        }
        var orders = await _orderService.GetUserOrdersAsync(_mockUserId);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(n => n.Name == User.Identity.Name);
                if (user != null)
                {
                    _mockUserId = user.Id;
                }
            }
            else
            {

                return RedirectToAction("Auth", "Account", "/Orders/Checkout");
            }
            var order = await _orderService.CreateOrderAsync(_mockUserId);
            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Cart");
        }
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = _context.Users.FirstOrDefault(n => n.Name == User.Identity.Name);
            if (user != null)
            {
                _mockUserId = user.Id;
            }
        }
        else
        {

            return RedirectToAction("Auth", "Account", "/Orders/Checkout");
        }
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null || order.Userid != _mockUserId) return NotFound();
        return View(order);
    }
}

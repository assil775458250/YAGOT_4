using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YAGOT_2._0.Models;
using YAGOT_2._0.Services;


namespace Yagot.Controllers;

public class CartController : Controller
{
    private readonly NeondbContext _context; 
    public  int _mockUserId = 1; 
    private readonly CartService _cartService;

    public CartController(NeondbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
      
    }
    public int nextCartId()
    {
        return _context.Carts.Any() ? _context.Carts.Max(c => c.Id) + 1 : 1;
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

            return RedirectToAction("Auth", "Account","/Cart/Index");
        }
            var cart = await _cartService.GetCartAsync(_mockUserId);
        ViewBag.message = _cartService.MESSAGE;
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity)
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

            return RedirectToAction("Auth", "Account", "/Cart/Add");
        }
        await _cartService.AddToCartAsync(_mockUserId, productId, quantity);
        TempData["Message"] = _cartService.MESSAGE;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
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

            return RedirectToAction("Auth", "Account", "/Cart/Remove");
        }
        await _cartService.RemoveFromCartAsync(_mockUserId, cartItemId);
        TempData["Message"] = _cartService.MESSAGE;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Drawer()
    {
        if (User.Identity?.IsAuthenticated != true)
            return StatusCode(401);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == User.Identity!.Name);
        if (user == null)
            return StatusCode(401);

        var cart = await _cartService.GetCartAsync(user.Id);
        return PartialView("_CartDrawer", cart);
    }
}

using HairForm.Database;
using HairForm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HairForm.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderList()
        {
            var orders = await _context.Orders
                .Include(o => o.Accessories)
                .OrderByDescending(o => o.DateTime)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePayStatus(string orderId, bool isPaid)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.IsPaid = isPaid;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCompleted()
        {
            var completedOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Completed)
                .ToListAsync();

            if (completedOrders.Any())
            {
                _context.Orders.RemoveRange(completedOrders);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(OrderList));
        }
    }
}
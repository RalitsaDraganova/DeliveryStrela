using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryStrela.Data;
using Microsoft.AspNetCore.Identity;
using DeliveryStrela.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DeliveryStrela.Controllers
{
    public class OrdersDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public const string OrderSessionKey = "OrderId";

        public OrdersDetailsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        //GET: OrdersDetails
        public async Task<IActionResult> Index()
        {
            var orderId = GetOrderId();

            if (orderId == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var currentUser = _userManager.GetUserId(User);

            var applicationDbContext = _context.OrdersDetails
                .Include(p => p.Product)
                .Include(o => o.Order)
                .Where(x => (x.OrderId == orderId) &&
                            (x.Order.Final == false) &&
                            (x.Order.UserId == currentUser));

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Calculate(int orderId)
        {
            var currentUser = _userManager.GetUserId(User);
            var dbOrderList = _context.OrdersDetails
               .Include(p => p.Product)
               .Include(o => o.Order)
               .Where(x => (x.OrderId == orderId) &&
                           (x.Order.Final == false) &&
                           (x.Order.UserId == currentUser));
            decimal sum = 0;
            foreach (var item in dbOrderList)
            {
                sum += (item.Product.Price * item.Count);
            }
           
            Order order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }
            order.Final = true;
            order.Total = sum;
            order.OrderOn = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("OrderSessionKey");
            return RedirectToAction("Index", "Orders");
        }

        // GET: OrdersDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersDetails = await _context.OrdersDetails
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersDetails == null)
            {
                return NotFound();
            }

            return View(ordersDetails);
        }

        public int? GetOrderId()
        {
            return HttpContext.Session.GetInt32("OrderSessionKey");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }

            if (GetOrderId()==null)
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    OrderOn=DateTime.Now.Date
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSessionKey",order.Id); 
            }

            int shoppingCarId = (int)GetOrderId();
            var orderItem = await _context.OrdersDetails
                .SingleOrDefaultAsync(x => (x.ProductId == product.Id && x.OrderId == shoppingCarId));
            if (orderItem == null)
            {
                orderItem = new OrdersDetails()
                {
                    ProductId = product.Id,
                    Count = product.Quantity,
                    OrderId = (int)GetOrderId()
                };
                _context.OrdersDetails.Add(orderItem);
            }
            else 
            {
                orderItem.Count = orderItem.Count + product.Quantity;
                _context.OrdersDetails.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Products");
        }
        
        // GET: OrdersDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersDetails = await _context.OrdersDetails
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersDetails == null)
            {
                return NotFound();
            }

            return View(ordersDetails);
        }

        // POST: OrdersDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordersDetails = await _context.OrdersDetails.FindAsync(id);
            _context.OrdersDetails.Remove(ordersDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersDetailsExists(int id)
        {
            return _context.OrdersDetails.Any(e => e.Id == id);
        }
    }
}

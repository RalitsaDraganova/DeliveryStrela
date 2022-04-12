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

namespace DeliveryStrela.Controllers
{
    public class OrdersDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersDetailsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrdersDetails
        public async Task<IActionResult> Index(int? id)
        {
            List<OrdersDetails> applicationDbContext = await _context.OrdersDetails
               .Include(o => o.Product).ToListAsync();
            if (id != null)
            {
                applicationDbContext = applicationDbContext
                .Where(x => x.OrderId == id).ToList();
            }

            return View(applicationDbContext);
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

        // GET: OrdersDetails/Create
        public async Task<IActionResult> Create(ProductVM product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }

            OrdersDetails modelToDb;
            if (product.IdUser.CompareTo(_userManager.GetUserId(User)) == 0)
            {
                modelToDb = new OrdersDetails()
                {
                    ProductId = product.Id,
                    Count = product.Count,
                    OrderId = Convert.ToInt32(ViewData["orderId"])
            };
            }
            else
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    OrderOn = DateTime.Now
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                ViewData["orderId"] = order.Id;
                modelToDb = new OrdersDetails()
                {
                    ProductId = product.Id,
                    Count = product.Count,
                    OrderId= order.Id
                };
            }
            _context.OrdersDetails.Add(modelToDb);
            await _context.SaveChangesAsync();

            //return Content("OK");
            return RedirectToAction(nameof(Index), new { id = modelToDb.OrderId });

        }

        // POST: OrdersDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,IdOrder,IdProduct,Count")] OrdersDetails ordersDetails)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(ordersDetails);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ordersDetails.IdProduct);
        //    return View(ordersDetails);
        //}

        // GET: OrdersDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersDetails = await _context.OrdersDetails.FindAsync(id);
            if (ordersDetails == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ordersDetails.ProductId);
            return View(ordersDetails);
        }

        // POST: OrdersDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdOrder,IdProduct,Count")] OrdersDetails ordersDetails)
        {
            if (id != ordersDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordersDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersDetailsExists(ordersDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ordersDetails.ProductId);
            return View(ordersDetails);
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

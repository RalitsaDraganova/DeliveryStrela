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
            //започва актуализиране на таблицата Orders /total=....; final=true
            Order order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Final = true;
            order.Total = sum;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            //изтрива ОРДЕРид от сесията
            HttpContext.Session.Remove("OrderSessionKey");

            return Content("SUM = " + sum.ToString());
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

        //метод за взимане на информация от сесията за патребителя
        public int? GetOrderId()
        {
            return HttpContext.Session.GetInt32("OrderSessionKey");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM product)
        {
            if (!ModelState.IsValid)//pri greshka
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }

            if (GetOrderId()==null)//ако потребителят няма поръчка досега от влизането си 
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    OrderOn=DateTime.Now.Date
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSessionKey",order.Id); //създаване на запис в сесията за потербителя с номер на поръчка
            }

            //ако потребителят вече е направил поръчка 
            int shoppingCarId = (int)GetOrderId();
            var orderItem = await _context.OrdersDetails
                .SingleOrDefaultAsync(x => (x.ProductId == product.Id && x.OrderId == shoppingCarId));
            if (orderItem == null)//ако поръчва друг/нов продукт се записва в orderdetails
            {
                orderItem = new OrdersDetails()
                {
                    ProductId = product.Id,
                    Count = product.Count,
                    OrderId = (int)GetOrderId()
                };
                _context.OrdersDetails.Add(orderItem);
            }
            else //ако избира вече поръчан продукт се увеличава количеството му
            {
                orderItem.Count = orderItem.Count + product.Count;
                _context.OrdersDetails.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Products");//kude se vrushta
        }
            
        //// GET: OrdersDetails/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var ordersDetails = await _context.OrdersDetails.FindAsync(id);
        //    if (ordersDetails == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ordersDetails.ProductId);
        //    return View(ordersDetails);
        //}

        //// POST: OrdersDetails/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,IdOrder,IdProduct,Count")] OrdersDetails ordersDetails)
        //{
        //    if (id != ordersDetails.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(ordersDetails);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrdersDetailsExists(ordersDetails.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ordersDetails.ProductId);
        //    return View(ordersDetails);
        //}

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

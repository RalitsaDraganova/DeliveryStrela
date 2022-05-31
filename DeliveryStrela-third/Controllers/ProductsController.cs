using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryStrela.Data;
using DeliveryStrela.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DeliveryStrela.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public async Task<IActionResult> FilterMain()
        {
            var product = _context.Products
            .Where(product => product.Category.Equals(CategoryKind.Main));
            return View(await product.ToListAsync());
        }

        public async Task<IActionResult> FilterAppetizer()
        {
            var product = _context.Products            
            .Where(product => product.Category.Equals(CategoryKind.Appetizer));
            return View(await product.ToListAsync());
        }

        public async Task<IActionResult> FilterDessert()
        {
            var product = _context.Products
            .Where(product => product.Category.Equals(CategoryKind.Dessert));
            return View(await product.ToListAsync());
        }

        public async Task<IActionResult> FilterGrill()
        {
            var product = _context.Products
            .Where(product => product.Category.Equals(CategoryKind.Grill));
            return View(await product.ToListAsync());
        }

        public async Task<IActionResult> FilterBread()
        {
            var product = _context.Products
            .Where(product => product.Category.Equals(CategoryKind.Bread));
            return View(await product.ToListAsync());
        }

        public async Task<IActionResult>FilterDiet()
        {
            var diet = _context.Products.Where(p => p.Diet == true);
            return View(await diet.ToListAsync());
        }
        public async Task<IActionResult> FilterFoodForEveryone()
        {
            var diet = _context.Products.Where(p => p.Diet == false);
            return View(await diet.ToListAsync());
        }
        [Authorize]
    
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ProductVM productVM = new ProductVM()
            {
                CatalogNumber = product.CatalogNumber,
                Name = product.Name,
                Grams = product.Grams,
                Description = product.Description,
                Id = product.Id,
                Price = product.Price,
                Diet = product.Diet.ToString(),
                UserId = _userManager.GetUserId(User),
                Quantity = 1,
                Category = product.Category
            };
            return View(productVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CatalogNumber,Name,Diet,Grams,Category,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CatalogNumber,Name,Diet,Grams,Category,Price,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

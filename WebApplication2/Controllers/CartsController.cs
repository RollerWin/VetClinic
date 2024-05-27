using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartsController(StoreDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(this.User);
            var userServices = _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Service)
            .Include(c => c.User);

            return View(await userServices.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();

            if(cartItems.Any())
            {
                foreach(var cartItem in cartItems)
                {
                    var orderHistoryItem = new OrderHistory
                    {
                        UserId = cartItem.UserId,
                        ServiceId = cartItem.ServiceId,
                        TotalPrice = cartItem.TotalPrice,
                        OrderDateTime = DateTime.UtcNow
                    };

                    _context.OrderHistories.Add(orderHistoryItem);
                    _context.Carts.Remove(cartItem);
                }

                await _context.SaveChangesAsync();
            }
            
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts
                .Include(c => c.Service)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cart == null)
                return NotFound();

            return View(cart);
        }

        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Author");
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ServiceId,Count,TotalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Author", cart.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", cart.UserId);
            return View(cart);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Author", cart.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", cart.UserId);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ServiceId,Count,TotalPrice")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Author", cart.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", cart.UserId);
            return View(cart);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var cart = await _context.Carts
            //    .Include(c => c.Service)
            //    .Include(c => c.User)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (cart == null)
            //{
            //    return NotFound();
            //}

            //return View(cart);
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var cart = await _context.Carts.FindAsync(id);
        //    if (cart != null)
        //    {
        //        _context.Carts.Remove(cart);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}

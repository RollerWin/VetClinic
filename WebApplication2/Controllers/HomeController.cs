using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly StoreDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, StoreDbContext context, IMemoryCache memoryCache)
        {
            _logger = logger;
            this._userManager = userManager;
            _context = context;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            ViewData["user"] = _userManager.GetUserId(this.User);
            return View();
        }

        public IActionResult Privacy() => View();

        public async Task<IActionResult> Catalog()
        {
            int cookieVisitCount = GetVisitCountFromCookie();
            cookieVisitCount++;
            SetVisitCountCookie(cookieVisitCount);
            DateTime lastVisit = GetLastVisitFromCookie();
            SetLastVisitCookie();
            ViewData["cookieVisitCount"] = cookieVisitCount;
            ViewData["LastVisit"] = lastVisit;

            int trackingTimeInMinutes = 30;
            int visitCount = _memoryCache.GetOrCreate<int>("VisitCount", entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(trackingTimeInMinutes));
                return 0;
            });

            visitCount++;
            _memoryCache.Set("VisitCount", visitCount);
            ViewData["VisitCount"] = visitCount;
            ViewData["TrackingTimeInMinutes"] = trackingTimeInMinutes;

            var viewModel = await _context.Services.ToListAsync();
            return View(viewModel);
        }

        [Authorize]
        [HttpPost, ActionName("Catalog")]
        public async Task<IActionResult> Catalog(int serviceId)
        {
            var book = await _context.Services.FindAsync(serviceId);

            //if (book == null)
            //    return NotFound();

            var userId = _userManager.GetUserId(this.User);

            var cartItem = await _context.Carts
                .Where(c => c.UserId == userId && c.ServiceId == serviceId)
                .FirstOrDefaultAsync();

            if (cartItem == null)
            {
                var newCartItem = new Cart
                {
                    UserId = userId,
                    ServiceId = serviceId,
                    TotalPrice = book.Price
                };

                _context.Carts.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Catalog));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private DateTime GetLastVisitFromCookie()
        {
            if (Request.Cookies.ContainsKey("LastVisit"))
            {
                return Convert.ToDateTime(Request.Cookies["LastVisit"]);
            }
            return DateTime.Now;
        }

        private void SetLastVisitCookie()
        {
            Response.Cookies.Append("LastVisit", DateTime.Now.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30)
            });
        }

        private int GetVisitCountFromCookie()
        {
            if (Request.Cookies.ContainsKey("VisitCount"))
            {
                return Convert.ToInt32(Request.Cookies["VisitCount"]);
            }
            return 0;
        }

        private void SetVisitCountCookie(int visitCount)
        {
            Response.Cookies.Append("VisitCount", visitCount.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30)
            });
        }
    }
}

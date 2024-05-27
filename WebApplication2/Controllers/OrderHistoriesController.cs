using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    public class OrderHistoriesController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderHistoriesController(StoreDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(this.User);
            var storeDbContext = _context.OrderHistories
                .Where(c => c.UserId == userId)
                .Include(o => o.Service)
                .Include(o => o.User);

            return View(await storeDbContext.ToListAsync());
        }
    }
}

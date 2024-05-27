using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ServicesController : Controller
    {
        //private readonly StoreDbContext _context;
        private IRegisterRepository _context;

        //public BooksController(StoreDbContext context) => _context = context;
        public ServicesController(IRegisterRepository context) => _context = context;

        [Authorize(Roles = "admin")]
        //public async Task<IActionResult> Index() => View(await _context.Books.ToListAsync());
        public async Task<IActionResult> Index() => View(await _context.ListAsync());

        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
                return NotFound();

            //var book = await _context.Books
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var book = await _context.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Publisher,PublicationYear,ISBN,Genre,Description,Price,ImagePath,Count")] Service book)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(book);
                //await _context.SaveChangesAsync();
                _context.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
                return NotFound();

            //var book = await _context.Books.FindAsync(id);
            var book = await _context.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Publisher,PublicationYear,ISBN,Genre,Description,Price,ImagePath,Count")] Service book)
        {
            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(book);
                    //await _context.SaveChangesAsync();
                    _context.UpdateAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!BookExists(book.Id))
                    //    return NotFound();
                    //else
                    //    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
                return NotFound();

            //var book = await _context.Books
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var book = await _context.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var book = await _context.Books.FindAsync(id);
            await _context.DeleteAsync(id);

            //if (book != null)
            //_context.Books.Remove(book);

            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //private bool BookExists(int id) => _context.Books.Any(e => e.Id == id);
    }
}

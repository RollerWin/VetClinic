using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApplication2.Data;

namespace WebApplication2.Models
{
    public interface IRegisterRepository
    {
        Task<Service> GetByIdAsync(int id);
        Task<List<Service>> ListAsync();
        Task CreateAsync(Service Book);
        Task UpdateAsync(Service Book);
        Task DeleteAsync(int id);
    }

    public class RegisterRepository : IRegisterRepository
    {
        private readonly StoreDbContext _context;

        public RegisterRepository(StoreDbContext dbContext) => _context = dbContext;

        public Task<Service> GetByIdAsync(int id) => _context.Services.FirstOrDefaultAsync(s => s.Id == id);

        public Task<List<Service>> ListAsync() => _context.Services.ToListAsync();

        public Task CreateAsync(Service Book)
        {
            _context.Services.Add(Book);
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(Service Book)
        {
            _context.Entry(Book).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var r = await GetByIdAsync(id);
            _context.Services.Remove(r);
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
    }
}

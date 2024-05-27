using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().ToTable("IdentityUser");
        }

        //public class EmailSender : IEmailSender
        //{
        //    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //    {
        //        // Здесь реализуйте отправку электронного письма
        //        return Task.CompletedTask;
        //    }
        //}
    }
}

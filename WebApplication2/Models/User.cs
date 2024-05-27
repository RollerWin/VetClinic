using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}

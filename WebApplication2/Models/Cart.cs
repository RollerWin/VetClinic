using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [ForeignKey("AspNetUsers")]
        [Required(ErrorMessage = "Идентификатор пользователя обязателен")]
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Required(ErrorMessage = "Идентификатор книги обязателен")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required(ErrorMessage = "Общая цена обязательна")]
        [Range(0.01, 1000000, ErrorMessage = "Общая цена должна быть в диапазоне от 0.01 до 1 000 000")]
        public decimal TotalPrice { get; set; }
    }
}

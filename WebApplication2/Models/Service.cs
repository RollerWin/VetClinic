using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Service
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Название книги обязательно")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Картинка обязательна")]
        public string ImagePath { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 1000000, ErrorMessage = "Цена должна быть в диапазоне от 0.01 до 1 000 000")]
        public decimal Price { get; set; }
    }
}

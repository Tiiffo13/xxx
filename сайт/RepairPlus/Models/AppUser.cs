using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RepairPlus.Models
{
    public class AppUser : IdentityUser
    {

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public ICollection<Order> Orders { get; set; }
    }
}

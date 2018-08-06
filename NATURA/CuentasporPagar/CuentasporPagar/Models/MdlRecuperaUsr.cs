using System.ComponentModel.DataAnnotations;

namespace CuentasporPagar.Models
{
    public class MdlRecuperaUsr
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
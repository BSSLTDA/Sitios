using System.ComponentModel.DataAnnotations;

namespace PanelAsistentes.Models
{
    public class MdlLogin
    {
        [Required]
        [Display(Name = "Usuario")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
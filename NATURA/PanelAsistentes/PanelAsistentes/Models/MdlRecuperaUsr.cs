using System.ComponentModel.DataAnnotations;

namespace PanelAsistentes.Models
{
    public class MdlRecuperaUsr
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
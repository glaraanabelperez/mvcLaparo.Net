using System.ComponentModel.DataAnnotations;

namespace mvc.Laparoscopy.Models
{
    public class ContactoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(1000)]
        public string Mensaje { get; set; }
    }
}

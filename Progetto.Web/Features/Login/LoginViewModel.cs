using System.ComponentModel.DataAnnotations;

namespace Progetto.Web.Features.Login
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string TipoAccesso { get; set; }
    }
}

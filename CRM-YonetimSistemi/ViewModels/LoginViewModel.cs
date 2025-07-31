using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; } 

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string? Password { get; set; }

        [Display(Name = "Beni hatırla?")]
        public bool RememberMe { get; set; }
    }
}

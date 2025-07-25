using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.ViewModels
{
    // Kullanıcıları listelerken kullanılacak model
    public class UserViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }

    // Kullanıcı rol düzenleme sayfasında kullanılacak model
    public class EditUserViewModel
    {
        public string? UserId { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }

        // Sistemdeki tüm roller
        public List<RoleViewModel>? Roles { get; set; }
    }

    public class RoleViewModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    // Şifre değiştirme sayfasında kullanılacak model
    public class ChangePasswordViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }
    }
}

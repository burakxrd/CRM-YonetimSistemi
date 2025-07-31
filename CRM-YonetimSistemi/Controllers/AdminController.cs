using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMYonetimSistemi.ViewModels;

namespace CRMYonetimSistemi.Controllers
{
    // Bu controller'a sadece "Admin" rolündeki kullanıcılar erişebilir.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Kullanıcı Yönetim Paneli Ana Sayfası
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                userViewModels.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return View(userViewModels);
        }

        #region Create User
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                TempData["ErrorMessage"] = "Kullanıcı adı, şifre ve rol alanları boş olamaz.";
                return View();
            }

            // IdentityUser oluşturulurken UserName null olamaz.
            var user = new IdentityUser { UserName = username, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
        #endregion

        #region Edit User Roles
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user?.UserName == null) // Hem user'ın hem de UserName'in null olmadığını kontrol et
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName, // Null kontrolü yapıldığı için güvenli.
                Roles = new List<RoleViewModel>()
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                if (role.Name != null) // Rol adının null olmadığını kontrol et
                {
                    model.Roles.Add(new RoleViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        IsSelected = userRoles.Contains(role.Name)
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId!);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // model.Roles null ise boş bir koleksiyon kullanarak hatayı önle (Null-coalescing operator '??')
            var selectedRoles = model.Roles?.Where(r => r.IsSelected).Select(r => r.RoleName!)
                                ?? Enumerable.Empty<string>();

            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Kullanıcı rolleri kaldırılırken bir hata oluştu.");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı rolleri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        #endregion

        #region Change User Password
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user?.UserName == null)
            {
                return NotFound();
            }
            var model = new ChangePasswordViewModel { UserId = user.Id, UserName = user.UserName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId!);
            if (user?.UserName == null)
            {
                return NotFound();
            }

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, model.NewPassword!);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"{user.UserName} kullanıcısının şifresi başarıyla değiştirildi.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        #endregion

        #region Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // user.UserName null olabileceği için ?. (null-conditional) operatörü ile güvenli kontrol
            if (user.UserName?.ToLower() == "superadmin")
            {
                TempData["ErrorMessage"] = "Süper admin kullanıcısı silinemez.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}

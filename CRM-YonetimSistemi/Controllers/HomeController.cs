using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CRMYonetimSistemi.Models;
using CRMYonetimSistemi.ViewModels; 

namespace CRMYonetimSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(SignInManager<IdentityUser> signInManager, ILogger<HomeController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Eðer kullanýcý zaten giriþ yapmýþsa, onu direkt kontrol paneline yönlendir.
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            // Deðilse, giriþ sayfasýna yönlendir.
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Giriþ iþlemi model.Email yerine model.UserName kullanýlarak yapýlýyor.
                var result = await _signInManager.PasswordSignInAsync(model.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanýcý giriþ yaptý.");
                    return RedirectToAction("Index", "Dashboard");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Kullanýcý hesabý kilitlendi.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz giriþ denemesi.");
                    return View(model);
                }
            }

            // Model geçerli deðilse, formu hatalarla birlikte tekrar göster.
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Kullanýcý çýkýþ yaptý.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

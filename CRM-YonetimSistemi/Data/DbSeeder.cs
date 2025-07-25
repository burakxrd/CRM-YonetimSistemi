using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CRMYonetimSistemi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndSuperAdminAsync(IServiceProvider services)
        {
            // Gerekli servisleri al
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Rol ve süper admin tohumlama işlemi başlıyor.");

            // Gerekli rollerin ("Admin", "User") varlığını kontrol et, yoksa oluştur.
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    logger.LogInformation($"'{roleName}' rolü başarıyla oluşturuldu.");
                }
            }

            // Süper admin kullanıcısını sadece bir kez oluştur
            var superAdminUserName = "KullanıcıAdınıBurayaGirin";
            var superAdmin = await userManager.FindByNameAsync(superAdminUserName);

            if (superAdmin == null)
            {
                superAdmin = new IdentityUser
                {
                    UserName = superAdminUserName
                };

                // LÜTFEN BU ŞİFREYİ İLK GİRİŞTEN SONRA DEĞİŞTİRİN!
                string superAdminPassword = "ŞifreyiBurayaGirin";
                var createResult = await userManager.CreateAsync(superAdmin, superAdminPassword);

                if (createResult.Succeeded)
                {
                    logger.LogInformation($"'{superAdminUserName}' kullanıcısı şifre ile oluşturuldu.");
                    await userManager.AddToRoleAsync(superAdmin, "Admin");
                    logger.LogInformation($"'{superAdminUserName}' kullanıcısı 'Admin' rolüne atandı.");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        logger.LogError("Süper admin oluşturulurken hata: {Error}", error.Description);
                    }
                }
            }
            else
            {
                logger.LogInformation($"'{superAdminUserName}' kullanıcısı zaten veritabanında mevcut.");
            }
        }
    }
}

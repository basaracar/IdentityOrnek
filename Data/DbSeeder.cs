using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOrnek.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityOrnek.Data
{
    public static class DbSeeder
    {
        public static async Task RoleEkle(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            //rolleri oluşturma
            string[] roller = { "Admin", "Member" };
            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }
            //Varsayılan Kullanıcı oluştur
            var adminMail = "admin@proje.com";
            var adminUser = await userManager.FindByEmailAsync(adminMail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminMail,
                    Email = adminMail,
                    Ad = "Başar",
                    Soyad = "ACAROĞLU",
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddClaimAsync(newAdmin, new Claim("TamAd", newAdmin.Ad + " " + newAdmin.Soyad));
                    await userManager.AddToRoleAsync(newAdmin,"Admin");
                }


            }
        }
    }
}
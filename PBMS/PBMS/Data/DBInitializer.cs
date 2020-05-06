using Microsoft.AspNetCore.Identity;
using PBMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBMS.Data
{
    public class DBInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            //InitializeContact(context);
            await InitializeRole(roleManager);
            await InitializeUserandAssignRole(userManager);
        }

        //private static void InitializeContact(ApplicationDbContext context)
        //{
        //    var contact = new Contact()
        //    {
        //        Name = "Admin",
        //        Address = "Head Office",
        //        Email = "admin@gmail.com",
        //        Number = "01700000000",
        //        Occupation = "Employee"
        //    };

        //    context.Add(contact);
        //    context.SaveChanges();
        //}

        private static async Task InitializeRole(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        private static async Task InitializeUserandAssignRole(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByNameAsync("admin@gmail.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com"
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, "Admin2@");
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}

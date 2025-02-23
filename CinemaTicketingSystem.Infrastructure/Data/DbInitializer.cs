using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

                if (! _roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    // Create Roles
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();

                    // Create Admin User
                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "m.rasoulinejad@gmail.com",
                        NormalizedUserName = "M.RASOULINEJAD@GMAIL.COM",
                        Email = "m.rasoulinejad@gmail.com",
                        NormalizedEmail = "M.RASOULINEJAD@GMAIL.COM",
                        FirstName = "Max",
                        LastName = "Roslin",
                        EmailConfirmed = true,
                        PhoneNumber = "1234567890",

                    }, "Admin123!").GetAwaiter().GetResult();

                    ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "m.rasoulinejad@gmail.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}

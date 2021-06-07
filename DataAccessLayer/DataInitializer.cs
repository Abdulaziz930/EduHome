using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Data;
using EduHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduHome.DataAccessLayer
{
    public class DataInitializer
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataInitializer(AppDbContext db,UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedDataAsync()
        {
            await _db.Database.MigrateAsync();

            #region Role Seed

            var roles = new List<string>
            {
                RoleConstants.AdminRole,
                RoleConstants.CourseModeratorRole,
                RoleConstants.MemberRole
            };

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                    continue;

                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            #endregion

            #region User Seed

            var user = new User
            {
                Email = "admin@gmail.com",
                UserName = "Admin",
                FullName = "Admin Admin"
            };

            if(await _userManager.FindByEmailAsync(user.Email) == null)
            {
                await _userManager.CreateAsync(user, "Admin@123");
                await _userManager.AddToRoleAsync(user, RoleConstants.AdminRole);
            }

            #endregion
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Data;
using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.AdminRole)]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _db;

        public UserController(UserManager<User> userManager,SignInManager<User> signInManager,AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var dbUsers = new List<User>();

            if (User.Identity.IsAuthenticated)
            {
                dbUsers = await _userManager.Users.Where(x => x.UserName != User.Identity.Name).ToListAsync();   
            }

            var users = new List<UserViewModel>();

            foreach (var dbUser in dbUsers)
            {
                var user = new UserViewModel
                {
                    Id = dbUser.Id,
                    Fullname = dbUser.FullName,
                    UserName = dbUser.UserName,
                    Email = dbUser.Email,
                    Role = (await _userManager.GetRolesAsync(dbUser)).FirstOrDefault(),
                    IsActive = dbUser.IsActive
                };

                users.Add(user);
            }

            return View(users);
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var courses = await _db.Courses.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Courses = courses;

            var changeRoleViewModel = new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles = GetRoles()
            };

            return View(changeRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id,ChangeRoleViewModel changeRoleViewModel,int courseId,string role)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var courses = await _db.Courses.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Courses = courses;

            var dbChangeRoleViewModel = new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles = GetRoles()
            };

            if (!ModelState.IsValid)
            {
                return View(dbChangeRoleViewModel);
            }

            if(role.ToLower() == "CourseModerator".ToLower())
            {
                if (courseId == 0)
                {
                    ModelState.AddModelError("", "Please select category.");
                    return View();
                }

                var dbCourse = await _db.Courses.Where(x => x.IsDeleted == false)
                    .FirstOrDefaultAsync(x => x.Id == courseId);
                if (dbCourse == null)
                    return NotFound();


                dbCourse.UserId = user.Id;
                List<Course> courseList = new List<Course>();
                courseList.Add(dbCourse);
                user.Courses = courseList;
            }

            string oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            string newRole = changeRoleViewModel.Role;
            if(oldRole != newRole)
            {
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Some problem exist");
                    return View(dbChangeRoleViewModel);
                }

                var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Some problem exist");
                    return View(dbChangeRoleViewModel);
                }
            }

            await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public List<string> GetRoles()
        {
            List<string> roles = new List<string>();

            roles.Add(RoleConstants.AdminRole);
            roles.Add(RoleConstants.CourseModeratorRole);
            roles.Add(RoleConstants.MemberRole);

            return roles;
        }
    }
}

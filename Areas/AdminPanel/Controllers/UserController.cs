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

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Users.Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

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

        #region Change Role

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
        public async Task<IActionResult> ChangeRole(string id,ChangeRoleViewModel changeRoleViewModel,List<int> coursesId,string role)
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

            if(role.ToLower() == RoleConstants.CourseModeratorRole.ToLower())
            {
                if(coursesId.Count == 0)
                {
                    ModelState.AddModelError("", "Please select category.");
                    return View(dbChangeRoleViewModel);
                }
                foreach (var courseId in coursesId)
                {
                    var dbCourses = await _db.Courses.Where(x => x.IsDeleted == false && x.Id == courseId)
                        .ToListAsync();
                    if (dbCourses == null)
                        return NotFound();

                    List<Course> courseList = new List<Course>();


                    foreach (var dbCourse in dbCourses)
                    {
                        dbCourse.UserId = user.Id;
                        if(user.Courses != null)
                        {
                            user.Courses.Add(dbCourse);
                        }
                        else
                        {
                            courseList.Add(dbCourse);
                        }
                    }
                    if(user.Courses == null)
                    {
                        user.Courses = courseList;
                    }
                }
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

            if(newRole != RoleConstants.CourseModeratorRole)
            {
                foreach (var course in user.Courses)
                {
                    course.UserId = null;
                }
            }

            await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Activity

        public async Task<IActionResult> Activity(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            var courses = await _db.Courses.Where(x => x.IsDeleted == false).ToListAsync();

            if (user == null)
                return NotFound();

            if (user.IsActive)
            {
                user.IsActive = false;
                if((await _userManager.GetRolesAsync(user)).FirstOrDefault() == RoleConstants.CourseModeratorRole)
                {
                    foreach (var course in courses)
                    {
                        if(course.UserId == user.Id)
                        {
                            course.UserId = null;
                        }
                    }
                }
            }
            else
            {
                user.IsActive = true;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        #endregion

        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser == null)
                return NotFound();

            var courses = await _db.Courses.Where(x => x.IsDeleted == false && x.UserId == id).ToListAsync();

            var userViewModel = new UserViewModel
            {
                Id = dbUser.Id,
                Fullname = dbUser.FullName,
                UserName = dbUser.UserName,
                Email = dbUser.Email,
                Role = (await _userManager.GetRolesAsync(dbUser)).FirstOrDefault(),
                IsActive = dbUser.IsActive,
                Courses = courses
            };

            return View(userViewModel);
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

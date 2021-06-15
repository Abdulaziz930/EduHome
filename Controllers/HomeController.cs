using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;

        public HomeController(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _db.Sliders.ToListAsync();
            var events = await _db.Events.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Take(4).ToListAsync();

            var homeViewModel = new HomeViewModel
            {
                Sliders = sliders,
                Events = events
            };

            return View(homeViewModel);
        }

        #region GlobalSearch

        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return NotFound();
            }

            var searchViewModel = new SearchViewModel
            {
                Blogs = await _db.Blogs.Where(x => x.IsDeleted == false && x.Title.Contains(search.ToLower()))
                                .OrderByDescending(x => x.LastModification).Take(4).ToListAsync(),
                Courses = await _db.Courses.Where(x => x.IsDeleted == false && x.Name.Contains(search.ToLower()))
                                .OrderByDescending(x => x.LastModificationDate).Take(4).ToListAsync(),
                Events = await _db.Events.Where(x => x.IsDeleted == false && x.Title.Contains(search.ToLower()))
                                .OrderByDescending(x => x.LastModificationDate).Take(4).ToListAsync(),
                Teachers = await _db.Teachers.Where(x => x.IsDeleted == false && x.Name.Contains(search.ToLower())).Take(4).ToListAsync()
            };

            return PartialView("_GlobalSearchPartial", searchViewModel);
        }

        #endregion

        #region Subscribe

        public async Task<IActionResult> Subscribe(string email)
        {

            if (!User.Identity.IsAuthenticated)
            {
                if (String.IsNullOrEmpty(email))
                {
                    return Content("Email Reuired");
                }
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);
                if (!match.Success)
                    return Content("Please enter the e-mail correctly");
            }
            else
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                email = user.Email;
            }
           

            var isExists = await _db.Subscribes.AnyAsync(x => x.Email == email);
            if (isExists)
            {
                return Content("You are alredy subscribed");
            }

            var subscribe = new Subscribe
            {
                Email = email
            };


            await _db.Subscribes.AddAsync(subscribe);
            await _db.SaveChangesAsync();

            return Content("You have successfully subscribed");
        }

        #endregion
    }
}

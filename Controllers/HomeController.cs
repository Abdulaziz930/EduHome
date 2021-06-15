using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
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
    }
}

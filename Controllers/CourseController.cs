using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _db;

        public CourseController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            return View();
        }

        #region CourseDetail

        public async Task<IActionResult> CourseDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Where(x => x.IsDeleted == false).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
                return NotFound();

            var courses = await _db.Courses.Where(x => x.IsDeleted == false).ToListAsync();
            var courseCategory = await _db.CategoryCourses.Include(x => x.Category).ToListAsync();
            var categories = new List<Category>();
            foreach (var dbCourse in courses)
            {
                foreach (var item in courseCategory)
                {
                    if(dbCourse.Id == item.CourseId)
                    {
                        //if (categories.Contains(item.Category))
                        //{
                        //}
                        categories.Add(item.Category);
                    }
                }
            }

            var courseViewModel = new CourseViewModel
            {
                Course = course,
                Categories = categories
            };

            return View(courseViewModel);
        }

        #endregion

        #region CourseSearch

        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return NotFound();
            }

            var courses = await _db.Courses.Where(x => x.IsDeleted == false && x.Name.Contains(search.ToLower()))
                .OrderByDescending(x => x.LastModificationDate).ToListAsync();

            return PartialView("_CourseSearchPartial", courses);
        }

        #endregion
    }
}

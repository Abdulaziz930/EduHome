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

        public IActionResult Index(int? categoryId,int page = 1)
        {
            var courses = new List<Course>();

            if (categoryId == null)
            {
                ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 9);
                ViewBag.Page = page;

                if (ViewBag.PageCount < page || page <= 0)
                    return NotFound();

                return View(courses);
            }
            else
            {
                var categoryCourses = _db.CategoryCourses.Where(x => x.CategoryId == categoryId)
                    .Include(x=>x.Course).Where(x => x.Course.IsDeleted == false).OrderByDescending(x => x.Course.LastModificationDate);
                if (categoryCourses.Count() == 0)
                    return NotFound();
                foreach (var categoryCourse in categoryCourses)
                {
                    courses.Add(categoryCourse.Course);
                }
                return View(courses);
            }
            
        }

        #region CourseDetail

        public async Task<IActionResult> CourseDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Where(x => x.IsDeleted == false)
                .Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
                return NotFound();
            var courseViewModel = new CourseViewModel
            {
                Categories = await _db.Categories.Include(x => x.CategoryCourses.Where(y => y.Course.IsDeleted == false)).ThenInclude(x => x.Course).Where(x => x.IsDeleted == false).ToListAsync(),
                Course = course
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

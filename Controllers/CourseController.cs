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
            List<Course> courses = new List<Course>();

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
                IQueryable<CategoryCourse> categoryCourses = _db.CategoryCourses.Where(c => c.CategoryId == categoryId)
                    .Include(x=>x.Course).OrderByDescending(x => x.Course.LastModificationDate);
                foreach (CategoryCourse ct in categoryCourses)
                {
                    courses.Add(ct.Course);
                }
                return View(courses);
            }
            
        }

        #region CourseDetail

        public async Task<IActionResult> CourseDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Where(x => x.IsDeleted == false).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
                return NotFound();

            CourseViewModel courseViewModel = new CourseViewModel
            {
                Categories = await _db.Categories.Include(c => c.CategoryCourses).ToListAsync(),
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

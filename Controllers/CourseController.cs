using EduHome.DataAccessLayer;
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
            ViewBag.PageCount = Decimal.Ceiling(_db.Courses.Count() / 9);
            ViewBag.Page = page;

            return View();
        }

        public async Task<IActionResult> CourseDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Where(x => x.IsDeleted == false).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
                return NotFound();

            return View(course);
        }
    }
}

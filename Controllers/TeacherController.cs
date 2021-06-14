using EduHome.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _db;

        public TeacherController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling(_db.Courses.Count() / 9);
            ViewBag.Page = page;

            return View();
        }

        public async Task<IActionResult> TeacherDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await _db.Teachers.Where(x => x.IsDeleted == false)
                .Include(x => x.TeacherDetail).ThenInclude(x => x.Skill)
                .Include(x => x.TeacherDetail).ThenInclude(x => x.TeacherContactInfo)
                .Include(x => x.TeacherProfessions).ThenInclude(x => x.Profession)
                .Include(x => x.SocialMedias).FirstOrDefaultAsync(x => x.Id == id && x.TeacherDetail.IsDeleted == false);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }
    }
}

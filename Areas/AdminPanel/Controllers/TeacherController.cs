using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Areas.AdminPanel.Utils;
using EduHome.DataAccessLayer;
using EduHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class TeacherController : Controller
    {
        private readonly AppDbContext _db;

        public TeacherController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Math.Ceiling((decimal)(_db.Teachers.Count() / 5));
            ViewBag.Page = page;

            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(teachers);
        }

        public async Task<IActionResult> Create()
        {
            var professions = await _db.Professions.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Professions = professions;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher,int[] professionId)
        {
            var professions = await _db.Professions.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Professions = professions;

            if (teacher.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo field cannot be empty");
                return View();
            }

            if (!teacher.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "This is not a picture");
                return View();
            }

            if (!teacher.Photo.IsSizeAllowed(3000))
            {
                ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "teacher", teacher.Photo);
            teacher.Image = fileName;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var teacherProfessionList = new List<TeacherProfession>();
            foreach (var item in professionId)
            {
                var teacherProfession = new TeacherProfession
                {
                    ProfessionId = item,
                    TeacherId = teacher.Id
                };
                teacherProfessionList.Add(teacherProfession);
            }
            teacher.TeacherProfessions = teacherProfessionList;

            teacher.IsDeleted = false;

            await _db.AddRangeAsync(teacher, teacher.TeacherDetail, 
                teacher.TeacherDetail.Skill, teacher.TeacherDetail.TeacherContactInfo);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

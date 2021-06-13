using System;
using System.Collections.Generic;
using System.IO;
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

        #region Create

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

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await _db.Teachers.Include(x => x.TeacherDetail)
                .ThenInclude(x => x.Skill).Include(x => x.TeacherDetail)
                .ThenInclude(x => x.TeacherContactInfo).Include(x => x.TeacherProfessions)
                .ThenInclude(x => x.Profession).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (teacher == null)
                return NotFound();

            var professions = await _db.Professions.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Professions = professions;

            return View(teacher);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Teacher teacher,int[] professionId)
        {
            if (id == null)
                return NotFound();

            if (id != teacher.Id)
                return BadRequest();

            var dbTeacher = await _db.Teachers.Include(x => x.TeacherDetail)
                .ThenInclude(x => x.Skill).Include(x => x.TeacherDetail)
                .ThenInclude(x => x.TeacherContactInfo).Include(x => x.TeacherProfessions)
                .ThenInclude(x => x.Profession).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbTeacher == null)
                return NotFound();

            var professions = await _db.Professions.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Professions = professions;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileName = dbTeacher.Image;

            if (teacher.Photo != null)
            {
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

                var path = Path.Combine(Constants.ImageFolderPath, "teacher", dbTeacher.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "teacher", teacher.Photo);
            }

            var teacherProfessions = new List<TeacherProfession>();
            foreach (var item in professionId)
            {
                var teacherProfession = new TeacherProfession();
                teacherProfession.ProfessionId = item;
                teacherProfession.TeacherId = teacher.Id;
                teacherProfessions.Add(teacherProfession);
            }
            dbTeacher.TeacherProfessions = teacherProfessions;
            dbTeacher.Image = fileName;
            dbTeacher.Name = teacher.Name;
            dbTeacher.TeacherDetail = teacher.TeacherDetail;
            dbTeacher.TeacherDetail.Skill = teacher.TeacherDetail.Skill;
            dbTeacher.TeacherDetail.TeacherContactInfo = teacher.TeacherDetail.TeacherContactInfo;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await _db.Teachers.Include(x => x.TeacherDetail)
                .ThenInclude(x => x.Skill).Include(x => x.TeacherDetail)
                .ThenInclude(x => x.TeacherContactInfo).Include(x => x.SocialMedias)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteTeacher(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await _db.Teachers.Include(x => x.TeacherDetail)
                .ThenInclude(x => x.Skill).Include(x => x.TeacherDetail)
                .ThenInclude(x => x.TeacherContactInfo).Include(x => x.SocialMedias)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (teacher == null)
                return NotFound();

            teacher.IsDeleted = true;
            teacher.TeacherDetail.IsDeleted = true;
            teacher.TeacherDetail.Skill.IsDeleted = true;
            teacher.TeacherDetail.TeacherContactInfo.IsDeleted = true;
            foreach (var item in teacher.SocialMedias)
            {
                if(item.TeacherId == id)
                {
                    item.IsDeleted = true;
                }
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await _db.Teachers.Include(x => x.TeacherDetail)
                .ThenInclude(x => x.Skill).Include(x => x.TeacherDetail)
                .ThenInclude(x => x.TeacherContactInfo).Include(x => x.SocialMedias)
                .Include(x => x.TeacherProfessions).ThenInclude(x => x.Profession)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        #endregion
    }
}

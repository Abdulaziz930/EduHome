using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Areas.AdminPanel.Utils;
using EduHome.Data;
using EduHome.DataAccessLayer;
using EduHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.AdminRole)]
    public class CourseController : Controller
    {
        private readonly AppDbContext _db;

        public CourseController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var courses = await _db.Courses.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(courses);
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {

            if (course.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo field cannot be empty");
                return View();
            }

            if (!course.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "This is not a picture");
                return View();
            }

            if (!course.Photo.IsSizeAllowed(3000))
            {
                ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "course", course.Photo);

            course.Image = fileName;

            var isExist = await _db.Courses.AnyAsync(x => x.Name == course.Name && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a course with this name");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            course.CreationDate = DateTime.Now;
            course.LastModificationDate = DateTime.Now;

            await _db.AddRangeAsync(course, course.CourseDetail);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Include(x => x.CourseDetail)
                .Where(x => x.CourseDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Course course)
        {
            if (id == null)
                return NotFound();

            if (id != course.Id)
                return BadRequest();

            var dbCourse = await _db.Courses.Include(x => x.CourseDetail)
                .Where(x => x.CourseDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbCourse == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileName = dbCourse.Image;

            if (course.Photo != null)
            {
                if (!course.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "This is not a picture");
                    return View();
                }

                if (!course.Photo.IsSizeAllowed(3000))
                {
                    ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                    return View();
                }

                var path = Path.Combine(Constants.ImageFolderPath, "course", dbCourse.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "course", course.Photo);
            }

            var isExist = await _db.Courses.AnyAsync(x => x.Name == course.Name && x.Id != course.Id && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a course with this name");
                return View();
            }

            dbCourse.Image = fileName;
            dbCourse.Name = course.Name;
            dbCourse.Description = course.Description;
            dbCourse.CourseDetail = course.CourseDetail;
            dbCourse.LastModificationDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Include(x => x.CourseDetail)
                .Where(x => x.CourseDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Include(x => x.CourseDetail)
                .Where(x => x.CourseDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (course == null)
                return NotFound();

            course.IsDeleted = true;
            course.CourseDetail.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _db.Courses.Include(x => x.CourseDetail)
                .Where(x => x.CourseDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (course == null)
                return NotFound();

            return View(course);
        }

        #endregion
    }
}

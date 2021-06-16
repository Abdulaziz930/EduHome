using EduHome.Data;
using EduHome.DataAccessLayer;
using EduHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Categories.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var categories = await _db.Categories.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(categories);
        }

        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            var isExist = await _db.Categories.AnyAsync(x => x.Name == category.Name && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Category category)
        {
            if (id == null)
                return NotFound();

            if (id != category.Id)
                return NotFound();

            var dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbCategory == null)
                return NotFound();

            var isExist = await _db.Categories.AnyAsync(x => x.Name == category.Name && x.Id != category.Id && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            dbCategory.Name = category.Name;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (category == null)
                return NotFound();

            category.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Categories.Include(x => x.CategoryBlogs).ThenInclude(x => x.Blog)
                .Include(x => x.CategoryCourses).ThenInclude(x => x.Course)
                .Include(x => x.CategoryEvents).ThenInclude(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (category == null)
                return NotFound();

            return View(category);
        }

        #endregion
    }
}

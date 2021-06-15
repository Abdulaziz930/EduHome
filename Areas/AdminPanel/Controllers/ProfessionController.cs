using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ProfessionController : Controller
    {
        private readonly AppDbContext _db;

        public ProfessionController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Professions.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var professions = await _db.Professions.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(professions);
        }

        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Profession profession)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _db.AddAsync(profession);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var profession = await _db.Professions.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (profession == null)
                return NotFound();

            return View(profession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Profession profession)
        {
            if (id == null)
                return NotFound();

            if (id != profession.Id)
                return BadRequest();

            var dbProfession = await _db.Professions.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbProfession == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            dbProfession.Name = profession.Name;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var profession = await _db.Professions.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (profession == null)
                return NotFound();

            return View(profession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProfession(int? id)
        {
            if (id == null)
                return NotFound();

            var profession = await _db.Professions.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (profession == null)
                return NotFound();

            profession.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var profession = await _db.Professions.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (profession == null)
                return NotFound();

            return View(profession);
        }

        #endregion
    }
}

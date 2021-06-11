using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.DataAccessLayer;
using EduHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ProfessionController : Controller
    {
        private readonly AppDbContext _db;

        public ProfessionController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Math.Ceiling((decimal)(_db.Professions.Count() / 5));
            ViewBag.Page = page;

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
    }
}

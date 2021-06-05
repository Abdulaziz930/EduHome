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
    public class AboutController : Controller
    {
        private readonly AppDbContext _db;

        public AboutController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var about = _db.About.SingleOrDefault();
            if (about == null)
                return NotFound();

            return View(about);
        }

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var about = await _db.About.SingleOrDefaultAsync(x => x.Id == id);
            if (about == null)
                return NotFound();

            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,About about)
        {
            if (id == null)
                return NotFound();

            if (id != about.Id)
                return BadRequest();

            var dbAbout = await _db.About.SingleOrDefaultAsync(x => x.Id == id);
            if (dbAbout == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileName = dbAbout.Image;

            if (about.Photo != null)
            {
                if (!about.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "This is not a picture");
                    return View();
                }

                if (!about.Photo.IsSizeAllowed(3000))
                {
                    ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                    return View();
                }

                var path = Path.Combine(Constants.ImageFolderPath, "about", dbAbout.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "about", about.Photo);
            }

            dbAbout.Image = fileName;
            dbAbout.Title = about.Title;
            dbAbout.Description = about.Description;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var about = await _db.About.SingleOrDefaultAsync(x => x.Id == id);
            if (about == null)
                return NotFound();

            return View(about);
        }

        #endregion
    }
}

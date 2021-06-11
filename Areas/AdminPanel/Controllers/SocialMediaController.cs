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
    public class SocialMediaController : Controller
    {
        private readonly AppDbContext _db;

        public SocialMediaController(AppDbContext db)
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
            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Teachers = teachers;
            Dictionary<string, string> socialIcons = new Dictionary<string, string>();
            socialIcons.Add("Facebook", "zmdi zmdi-facebook");
            socialIcons.Add("Pinterest", "zmdi zmdi-pinterest");
            socialIcons.Add("Vimeo", "zmdi zmdi-vimeo");
            socialIcons.Add("Twitter", "zmdi zmdi-twitter");
            ViewBag.Icons = socialIcons;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SocialMedia socialMedia,int teacherId)
        {
            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Teachers = teachers;
            Dictionary<string, string> socialIcons = new Dictionary<string, string>();
            socialIcons.Add("Facebook", "zmdi zmdi-facebook");
            socialIcons.Add("Pinterest", "zmdi zmdi-pinterest");
            socialIcons.Add("Vimeo", "zmdi zmdi-vimeo");
            socialIcons.Add("Twitter", "zmdi zmdi-twitter");
            ViewBag.Icons = socialIcons;

            if (!ModelState.IsValid)
            {
                return View();
            }

            socialMedia.TeacherId = teacherId;
            socialMedia.IsDeleted = false;

            await _db.SocialMedias.AddAsync(socialMedia);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion
    }
}

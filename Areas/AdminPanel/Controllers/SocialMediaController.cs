using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
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
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Teachers.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false).Include(x => x.SocialMedias)
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
            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false).Include(x => x.SocialMedias).ToListAsync();
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

            foreach (var teacher in teachers)
            {
                if(teacher.Id == teacherId)
                {
                    foreach (var item in teacher.SocialMedias)
                    {
                        if(item.IsDeleted == false && item.Link == socialMedia.Link && item.Icon == socialMedia.Icon)
                        {
                            ModelState.AddModelError("", "is exists");
                            return View();
                        }
                    }
                }
            }

            socialMedia.TeacherId = teacherId;
            socialMedia.IsDeleted = false;

            await _db.SocialMedias.AddAsync(socialMedia);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var socialMedia = await _db.SocialMedias.Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (socialMedia == null)
                return NotFound();

            Dictionary<string, string> socialIcons = new Dictionary<string, string>();
            socialIcons.Add("Facebook", "zmdi zmdi-facebook");
            socialIcons.Add("Pinterest", "zmdi zmdi-pinterest");
            socialIcons.Add("Vimeo", "zmdi zmdi-vimeo");
            socialIcons.Add("Twitter", "zmdi zmdi-twitter");
            ViewBag.Icons = socialIcons;

            return View(socialMedia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,SocialMedia socialMedia)
        {
            if (id == null)
                return NotFound();

            if (id != socialMedia.Id)
                return BadRequest();

            var dbSocialMedia = await _db.SocialMedias.Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbSocialMedia == null)
                return NotFound();

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

            dbSocialMedia.Link = socialMedia.Link;
            dbSocialMedia.Icon = socialMedia.Icon;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var socialMedia = await _db.SocialMedias.Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (socialMedia == null)
                return NotFound();

            return View(socialMedia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSocialMedia(int? id)
        {
            if (id == null)
                return NotFound();

            var socialMedia = await _db.SocialMedias.Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (socialMedia == null)
                return NotFound();

            socialMedia.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var socialMedia = await _db.SocialMedias.Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (socialMedia == null)
                return NotFound();

            return View(socialMedia);
        }

        #endregion
    }
    
}

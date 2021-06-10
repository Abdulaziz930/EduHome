using System;
using System.Collections.Generic;
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
    public class EventController : Controller
    {
        private readonly AppDbContext _db;

        public EventController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling(_db.Events.Count() / 5);
            ViewBag.Page = page;

            var events = await _db.Events.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(events);
        }

        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (@event.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo field cannot be empty");
                return View();
            }

            if (!@event.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "This is not a picture");
                return View();
            }

            if (!@event.Photo.IsSizeAllowed(3000))
            {
                ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "event", @event.Photo);
            @event.Image = fileName;

            //eyni adda iki event olar?

            if (!ModelState.IsValid)
            {
                return View();
            }

            @event.CreationDate = DateTime.Now;
            @event.LastModificationDate = DateTime.Now;

            await _db.AddRangeAsync(@event, @event.EventDetail);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion
    }
}

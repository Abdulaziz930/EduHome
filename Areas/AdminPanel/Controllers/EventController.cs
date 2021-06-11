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

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Event @event)
        {
            if (id == null)
                return NotFound();

            if (id != @event.Id)
                return BadRequest();

            var dbEvent = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbEvent == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileName = dbEvent.Image;

            if (@event.Photo != null)
            {
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

                var path = Path.Combine(Constants.ImageFolderPath, "event", dbEvent.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "event", @event.Photo);
            }

            dbEvent.Image = fileName;
            dbEvent.Title = @event.Title;
            dbEvent.Venue = @event.Venue;
            dbEvent.StartTime = @event.StartTime;
            dbEvent.EndTime = @event.EndTime;
            dbEvent.EventDetail = @event.EventDetail;
            dbEvent.LastModificationDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteEvent(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (@event == null)
                return NotFound();

            @event.IsDeleted = true;
            @event.EventDetail.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Include(x => x.EventDetail)
                .Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Speaker)
                .Where(x => x.EventDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        #endregion
    }
}

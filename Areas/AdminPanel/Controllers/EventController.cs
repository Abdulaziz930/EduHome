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
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Events.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var events = await _db.Events.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(events);
        }

        #region Create

        public async Task<IActionResult> Create()
        {
            var categories = await _db.Categories.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event, int[] categoryId)
        {
            var categories = await _db.Categories.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Categories = categories;

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

            if (!ModelState.IsValid)
            {
                return View();
            }

            if(@event.StartTime > @event.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start date cannot be later than end date");
                return View();
            }

            if (categoryId.Length == 0)
            {
                ModelState.AddModelError("", "Please select category.");
                return View();
            }

            var categoryEventList = new List<CategoryEvent>();
            foreach (var item in categoryId)
            {
                var categoryEvent = new CategoryEvent
                {
                    CategoryId = item,
                    EventId = @event.Id
                };
                categoryEventList.Add(categoryEvent);
            }
            @event.CategoryEvents = categoryEventList;

            @event.CreationDate = DateTime.Now;
            @event.LastModificationDate = DateTime.Now;

            await _db.AddRangeAsync(@event, @event.EventDetail);
            await _db.SaveChangesAsync();

            var link = Url.Action("EventDetail", "Event", new { Area = "default" , @event.Id }, protocol: HttpContext.Request.Scheme);
            var message = $"<a href={link}>Click to see the new event</a>";
            var subscribes = await _db.Subscribes.ToListAsync();
            foreach (var sub in subscribes)
            {
                await EmailUtil.SendEmailAsync(sub.Email, message, $"A new event named {@event.Title} has been created");
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var categories = await _db.Categories.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Categories = categories;

            var @event = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false).Include(x => x.CategoryEvents)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Event @event, int[] categoryId)
        {
            if (id == null)
                return NotFound();

            if (id != @event.Id)
                return BadRequest();

            var categories = await _db.Categories.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Categories = categories;

            var dbEvent = await _db.Events.Include(x => x.EventDetail)
                .Where(x => x.EventDetail.IsDeleted == false).Include(x => x.CategoryEvents)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbEvent == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (@event.StartTime > @event.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start date cannot be later than end date");
                return View(@event);
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

            if (categoryId.Length == 0)
            {
                ModelState.AddModelError("", "Please select category.");
                return View();
            }

            var categoryEventList = new List<CategoryEvent>();
            foreach (var item in categoryId)
            {
                var categoryEvent = new CategoryEvent();
                categoryEvent.CategoryId = item;
                categoryEvent.EventId = @event.Id;
                categoryEventList.Add(categoryEvent);
            }
            dbEvent.CategoryEvents = categoryEventList;
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

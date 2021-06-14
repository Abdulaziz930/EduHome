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
    public class SpeakerController : Controller
    {
        private readonly AppDbContext _db;

        public SpeakerController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Speakers.Where(x => x.IsDeleted == false).Count() / 5);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var speakers = await _db.Speakers.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();

            return View(speakers);
        }

        #region Create

        public async Task<IActionResult> Create()
        {
            var events = await _db.Events.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Events = events;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Speaker speaker,int[] eventId)
        {
            var events = await _db.Events.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Events = events;

            if (!ModelState.IsValid)
            {
                return View();
            }

            foreach (var item in eventId)
            {
                if (events.All(x => x.Id != item))
                    return NotFound();
            }

            if (speaker.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo field cannot be empty");
                return View();
            }

            if (!speaker.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "This is not a picture");
                return View();
            }

            if (!speaker.Photo.IsSizeAllowed(3000))
            {
                ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "event", speaker.Photo);
            speaker.Image = fileName;


            var eventSpeakerList = new List<EventSpeaker>();
            foreach (var item in eventId)
            {
                var eventSpeaker = new EventSpeaker
                {
                    EventId = item,
                    SpeakerId = speaker.Id
                };
                eventSpeakerList.Add(eventSpeaker);
            }
            speaker.EventSpeakers = eventSpeakerList;

            speaker.IsDeleted = false;

            await _db.Speakers.AddAsync(speaker);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var events = await _db.Events.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Events = events;

            var speaker = await _db.Speakers.Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false); 
            if (speaker == null)
                return NotFound();

            return View(speaker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Speaker speaker, int[] eventId)
        {
            if (id == null)
                return NotFound();

            if (id != speaker.Id)
                return BadRequest();

            var events = await _db.Events.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Events = events;

            var dbSpeaker = await _db.Speakers.Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbSpeaker == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileName = dbSpeaker.Image;

            if (speaker.Photo != null)
            {
                if (!speaker.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "This is not a picture");
                    return View();
                }

                if (!speaker.Photo.IsSizeAllowed(3000))
                {
                    ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                    return View();
                }

                var path = Path.Combine(Constants.ImageFolderPath, "event", dbSpeaker.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "event", speaker.Photo);
            }
            
            var eventSpikers = new List<EventSpeaker>();
            foreach (var item in eventId)
            {
                var eventSpeaker = new EventSpeaker();
                eventSpeaker.EventId = item;
                eventSpeaker.SpeakerId = speaker.Id;
                eventSpikers.Add(eventSpeaker);
            }
            dbSpeaker.EventSpeakers = eventSpikers;
            dbSpeaker.Image = fileName;
            dbSpeaker.FullName = speaker.FullName;
            dbSpeaker.Profession = speaker.Profession;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var speaker = await _db.Speakers.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (speaker == null)
                return NotFound();

            return View(speaker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSpeaker(int? id)
        {
            if (id == null)
                return NotFound();

            var speaker = await _db.Speakers.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (speaker == null)
                return NotFound();

            speaker.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var speaker = await _db.Speakers.Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (speaker == null)
                return NotFound();

            return View(speaker);
        }

        #endregion
    }
}

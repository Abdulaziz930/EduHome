using EduHome.DataAccessLayer;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _db;

        public EventController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? categoryId,int page = 1)
        {
            var events = new List<Event>();

            if(categoryId == null)
            {
                ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 9);
                ViewBag.Page = page;

                if (ViewBag.PageCount < page || page <= 0)
                    return NotFound();

                var dbEvents = await _db.Events.Where(x => x.IsDeleted == false)
                    .OrderByDescending(x => x.LastModificationDate).Skip((page - 1) * 9).Take(9).ToListAsync();

                return View(dbEvents);
            }
            else
            {
                var categoryEvents = _db.CategoryEvents.Where(x => x.CategoryId == categoryId)
                    .Include(x => x.Event).OrderByDescending(x => x.Event.LastModificationDate);
                foreach (var categoryEvent in categoryEvents)
                {
                    events.Add(categoryEvent.Event);
                }
                return View(events);
            }
        }

        #region EventDetail

        public async Task<IActionResult> EventDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Where(x => x.IsDeleted == false)
                .Include(x => x.EventDetail).Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Speaker).FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            var eventViewModel = new EventViewModel
            {
                Categories = await _db.Categories.Include(x => x.CategoryEvents).Where(x => x.IsDeleted == false).ToListAsync(),
                Event = @event
            };

            return View(eventViewModel);
        }

        #endregion

        #region EventSearch

        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return NotFound();
            }

            var events = await _db.Events.Where(x => x.IsDeleted == false && x.Title.Contains(search.ToLower()))
                .OrderByDescending(x => x.LastModificationDate).ToListAsync();

            return PartialView("_EventSearchPartial", events);
        }

        #endregion
    }
}

using EduHome.DataAccessLayer;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            var events = await _db.Events.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Skip((page - 1) * 9).Take(9).ToListAsync();

            return View(events);
        }

        public async Task<IActionResult> EventDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _db.Events.Where(x => x.IsDeleted == false)
                .Include(x => x.EventDetail).Include(x => x.EventSpeakers)
                .ThenInclude(x => x.Speaker).FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }
    }
}

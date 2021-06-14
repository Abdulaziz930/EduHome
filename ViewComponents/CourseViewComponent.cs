using EduHome.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewComponents
{
    public class CourseViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public CourseViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(InvokeRequest invokeRequest)
        {
            

            var courses = await _db.Courses.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Skip((invokeRequest.SkipCount - 1) * invokeRequest.Count).Take(invokeRequest.Count).ToListAsync();

            return View(courses);
        }
    }
    public class InvokeRequest
    {
        public int Count { get; set; } = 9;
        public int SkipCount { get; set; } = 0;
    }
}

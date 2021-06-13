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

        public async Task<IViewComponentResult> InvokeAsync(int count = 9)
        {
            var courses = await _db.Courses.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModificationDate).Take(count).ToListAsync();

            return View(courses);
        }
    }
}

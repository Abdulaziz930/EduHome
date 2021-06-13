using EduHome.DataAccessLayer;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewComponents
{
    public class TestimonialAreaViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public TestimonialAreaViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var testimonial = await _db.Testimonials.FirstOrDefaultAsync(x => x.IsDeleted == false);

            return View(testimonial);
        }
    }
}

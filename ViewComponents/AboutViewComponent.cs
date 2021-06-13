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
    public class AboutViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public AboutViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about = await _db.About.FirstOrDefaultAsync();

            return View(about);
        }
    }
}

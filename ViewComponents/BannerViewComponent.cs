using EduHome.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewComponents
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public BannerViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string key)
        {
            var banner = await _db.Banners.FirstOrDefaultAsync(x => x.Key == key);

            return View(banner);
        }
    }
}

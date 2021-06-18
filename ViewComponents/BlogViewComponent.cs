using EduHome.Data;
using EduHome.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public BlogViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(InvokeRequest invokeRequest)
        {
            var blogs = await _db.Blogs.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModification).Skip((invokeRequest.SkipCount - 1) * (invokeRequest.Count))
                .Take(invokeRequest.Count).ToListAsync();

            return View(blogs);
        }
    }
}

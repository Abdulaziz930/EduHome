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

        public async Task<IViewComponentResult> InvokeAsync(int count = 9)
        {
            var blogs = await _db.Blogs.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModification).Take(count).ToListAsync();

            return View(blogs);
        }
    }
}

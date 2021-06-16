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
    public class PostViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public PostViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = await _db.Blogs.Where(x => x.IsDeleted == false).OrderByDescending(x => x.LastModification).Take(3).ToListAsync();

            return View(blogs);
        }
    }
}

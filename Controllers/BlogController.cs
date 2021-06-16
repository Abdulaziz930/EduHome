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
    public class BlogController : Controller
    {
        private readonly AppDbContext _db;

        public BlogController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? categoryId,int page = 1)
        {
            var blogs = new List<Blog>();

            if (categoryId == null)
            {
                ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Courses.Where(x => x.IsDeleted == false).Count() / 9);
                ViewBag.Page = page;

                if (ViewBag.PageCount < page || page <= 0)
                    return NotFound();

                return View(blogs);
            }
            else
            {
                var categoryBlogs = _db.CategoryBlogs.Where(x => x.CategoryId == categoryId)
                    .Include(x => x.Blog).Where(x => x.Blog.IsDeleted == false).OrderByDescending(x => x.Blog.LastModification);
                foreach (var categoryBlog in categoryBlogs)
                {
                    blogs.Add(categoryBlog.Blog);
                }
                return View(blogs);
            }
        }

        #region BlogDetail

        public async Task<IActionResult> BlogDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _db.Blogs.Where(x => x.IsDeleted == false)
                .Include(x => x.BlogDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (blog == null)
                return NotFound();

            var blogViewModel = new BlogViewModel
            {
                Categories = await _db.Categories.Include(x => x.CategoryBlogs.Where(y => y.Blog.IsDeleted == false)).ThenInclude(x => x.Blog).Where(x => x.IsDeleted == false).ToListAsync(),
                Blog = blog
            };

            return View(blogViewModel);
        }

        #endregion

        #region BlogSearch

        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return NotFound();
            }

            var blogs = await _db.Blogs.Where(x => x.IsDeleted == false && x.Title.Contains(search.ToLower()))
                .OrderByDescending(x => x.LastModification).ToListAsync();

            return PartialView("_BlogSearchPartial", blogs);
        }

        #endregion
    }
}

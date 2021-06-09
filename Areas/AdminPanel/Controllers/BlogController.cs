using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Areas.AdminPanel.Utils;
using EduHome.Data;
using EduHome.DataAccessLayer;
using EduHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.AdminRole)]
    public class BlogController : Controller
    {
        private readonly AppDbContext _db;

        public BlogController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling(_db.Blogs.Count() / 5);
            ViewBag.Page = page;

            var blogs = await _db.Blogs.Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.LastModification).Skip((page - 1) * 5).Take(5).ToListAsync();


            return View(blogs);
        }

        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (blog.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo field cannot be empty");
                return View();
            }

            if (!blog.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "This is not a picture");
                return View();
            }

            if (!blog.Photo.IsSizeAllowed(3000))
            {
                ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "blog", blog.Photo);

            blog.Image = fileName;

            var isExist = await _db.Blogs.AnyAsync(x => x.Title == blog.Title && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a blog with this name");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            blog.CreationDate = DateTime.Now;
            blog.LastModification = DateTime.Now;

            await _db.AddRangeAsync(blog, blog.BlogDetail);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _db.Blogs.Include(x => x.BlogDetail)
                .Where(x => x.BlogDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Blog blog)
        {
            if (id == null)
                return NotFound();

            if (id != blog.Id)
                return NotFound();

            var dbBlog = await _db.Blogs.Include(x => x.BlogDetail)
                .Where(x => x.BlogDetail.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (dbBlog == null)
                return NotFound();

            var fileName = dbBlog.Image;

            if (blog.Photo != null)
            {
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "This is not a picture");
                    return View();
                }

                if (!blog.Photo.IsSizeAllowed(3000))
                {
                    ModelState.AddModelError("Photo", "The size of the image you uploaded is 3 MB higher.");
                    return View();
                }

                var path = Path.Combine(Constants.ImageFolderPath, "blog", dbBlog.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                fileName = await FileUtil.GenerateFileAsync(Constants.ImageFolderPath, "blog", blog.Photo);
            }

            var isExist = await _db.Blogs.AnyAsync(x => x.Title == blog.Title && x.Id != blog.Id && x.IsDeleted == false);
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is a course with this name");
                return View();
            }

            dbBlog.Image = fileName;
            dbBlog.Title = blog.Title;
            dbBlog.BlogDetail = blog.BlogDetail;
            dbBlog.LastModification = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _db.Blogs.Include(x => x.BlogDetail)
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBlog(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _db.Blogs.Include(x => x.BlogDetail)
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (blog == null)
                return NotFound();

            blog.IsDeleted = true;
            blog.BlogDetail.IsDeleted = true;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _db.Blogs.Include(x => x.BlogDetail)
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        #endregion
    }
}

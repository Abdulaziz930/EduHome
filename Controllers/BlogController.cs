﻿using EduHome.DataAccessLayer;
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

        public IActionResult Index(int page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_db.Blogs.Where(x => x.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
                return NotFound();

            return View();
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

            return View(blog);
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
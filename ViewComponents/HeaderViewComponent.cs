using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.DataAccessLayer;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public HeaderViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var bio = await _db.Bios.FirstOrDefaultAsync();
            var contact = await _db.Contacts.FirstOrDefaultAsync();

            var layoutViewModel = new LayoutViewModel
            {
                Bio = bio,
                Contact = contact
            };

            return View(layoutViewModel);
        }
    }
}

using EduHome.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewComponents
{
    public class TeacherViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public TeacherViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 9)
        {
            var teachers = await _db.Teachers.Where(x => x.IsDeleted == false)
                .Include(x => x.TeacherProfessions).ThenInclude(x => x.Profession)
                .Include(x => x.SocialMedias).Take(count).ToListAsync();

            return View(teachers);
        }
    }
}

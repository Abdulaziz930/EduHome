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
    public class NoticeAreaViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public NoticeAreaViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 9)
        {
            var noticeBoards = await _db.NoticeBoards.Where(x => x.IsDeleted == false).ToListAsync();
            var videoTour = await _db.VideoTours.SingleOrDefaultAsync();

            var noticeAreaViewModel = new NoticeAreaViewModel
            {
                NoticeBoards = noticeBoards,
                VideoTour = videoTour
            };

            return View(noticeAreaViewModel);
        }
    }
}

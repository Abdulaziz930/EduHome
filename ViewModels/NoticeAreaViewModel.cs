using EduHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewModels
{
    public class NoticeAreaViewModel
    {
        public VideoTour VideoTour { get; set; }

        public List<NoticeBoard> NoticeBoards { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class NoticeBoard
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}

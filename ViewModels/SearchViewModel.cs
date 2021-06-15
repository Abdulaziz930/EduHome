using EduHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewModels
{
    public class SearchViewModel
    {
        public List<Blog> Blogs { get; set; }

        public List<Course> Courses { get; set; }

        public List<Teacher> Teachers { get; set; }

        public List<Event> Events { get; set; }
    }
}

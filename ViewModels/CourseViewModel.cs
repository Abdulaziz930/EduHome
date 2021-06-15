using EduHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }

        public List<Category> Categories { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CategoryCourse> CategoryCourses { get; set; }

        public ICollection<CategoryEvent> CategoryEvents { get; set; }

        public ICollection<CategoryBlog> CategoryBlogs { get; set; }
    }
}

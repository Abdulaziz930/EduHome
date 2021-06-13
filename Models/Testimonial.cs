using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class Testimonial
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Position { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }

    }
}

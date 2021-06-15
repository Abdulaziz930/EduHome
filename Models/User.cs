using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EduHome.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public string Image { get; set; }

        public List<Course> Courses { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

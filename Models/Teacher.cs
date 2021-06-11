using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EduHome.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<SocialMedia> SocialMedias { get; set; }

        public ICollection<TeacherProfession> TeacherProfessions { get; set; }

        public TeacherDetail TeacherDetail { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

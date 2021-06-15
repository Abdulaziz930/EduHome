using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class TeacherDetail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Degree { get; set; }

        [Required]
        public string Experience { get; set; }

        public string Hobbies { get; set; }

        [Required]
        public string Faculty { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        public Teacher Teacher { get; set; }

        public Skill Skill { get; set; }

        public TeacherContactInfo TeacherContactInfo { get; set; }
    }
}

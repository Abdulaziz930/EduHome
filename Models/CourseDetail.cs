using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class CourseDetail
    {
        public int Id { get; set; }

        [Required]
        public string AboutCourse { get; set; }

        [Required]
        public string HowToApply { get; set; }

        [Required]
        public string Certification { get; set; }

        [Required]
        public string LeaveReply { get; set; }

        [Required]
        public DateTime Starts { get; set; }

        [Required]
        public string Duration { get; set; }

        [Required]
        public string ClassDuration { get; set; }

        [Required]
        public string SkillLevel { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public int StundetCount { get; set; }

        [Required]
        public string Assesments { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}

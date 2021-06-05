using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class CourseDetail
    {
        public int Id { get; set; }

        public string AboutCourse { get; set; }

        public string HowToApply { get; set; }

        public string Certification { get; set; }

        public string LeaveReply { get; set; }

        public DateTime Starts { get; set; }

        public string Duration { get; set; }

        public string ClassDuration { get; set; }

        public string SkillLevel { get; set; }

        public string Language { get; set; }

        public int StundetCount { get; set; }

        public string Assesments { get; set; }

        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}

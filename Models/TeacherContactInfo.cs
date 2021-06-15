using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class TeacherContactInfo
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Skype { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("TeacherDetail")]
        public int TeacherDetailId { get; set; }

        public TeacherDetail TeacherDetail { get; set; }
    }
}

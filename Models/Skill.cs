using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public int LanguagePercent { get; set; }

        public int TeamLeaderPercent { get; set; }

        public int DevelopmentPercent { get; set; }

        public int DesingPercent { get; set; }

        public int InnovationPercent { get; set; }

        public int CommunicationPercent { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("TeacherDetail")]
        public int TeacherDetailId { get; set; }

        public TeacherDetail TeacherDetail { get; set; }
    }
}

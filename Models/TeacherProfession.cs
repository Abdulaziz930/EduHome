using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class TeacherProfession
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public Teacher Teacher { get; set; }

        public int ProfessionId { get; set; }

        public Profession Profession { get; set; }
    }
}

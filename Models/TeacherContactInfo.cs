﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class TeacherContactInfo
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Skype { get; set; }

        [ForeignKey("TeacherDetail")]
        public int TeacherDetailId { get; set; }

        public TeacherDetail TeacherDetail { get; set; }
    }
}

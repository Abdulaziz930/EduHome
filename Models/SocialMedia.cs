using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class SocialMedia
    {
        public int Id { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Icon { get; set; }

        public bool IsDeleted { get; set; }

        public int TeacherId { get; set; }

        public Teacher Teacher { get; set; }
    }
}

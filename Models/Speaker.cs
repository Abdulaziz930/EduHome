using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EduHome.Models
{
    public class Speaker
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Image { get; set; }

        [Required]
        public string Profession { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<EventSpeaker> EventSpeakers { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EduHome.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public DateTime  StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Venue { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModificationDate { get; set; }

        public bool IsDeleted { get; set; }

        public EventDetail EventDetail { get; set; }

        public ICollection<EventSpeaker> EventSpeakers { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string FirstNumber { get; set; }

        public string SecondNumber { get; set; }

        [Required]
        public string FirstMailAddress { get; set; }

        public string SecondMailAddress { get; set; }

        public string Address { get; set; }
    }
}

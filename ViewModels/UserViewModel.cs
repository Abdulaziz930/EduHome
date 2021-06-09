using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}

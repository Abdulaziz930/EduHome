using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Models;

namespace EduHome.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserName { get; set; }

        [Required]
        public string Role { get; set; }

        public List<string> Roles { get; set; }

        public Course Course { get; set; }
    }
}

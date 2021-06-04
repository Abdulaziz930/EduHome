using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Models;

namespace EduHome.ViewModels
{
    public class LayoutViewModel
    {
        public Bio Bio { get; set; }

        public Contact Contact { get; set; }
    }
}

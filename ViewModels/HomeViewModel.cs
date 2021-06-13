using EduHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }

        public About About { get; set; }
    }
}

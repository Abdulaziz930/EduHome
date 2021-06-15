using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Areas.AdminPanel.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public NavigationViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = new UserViewModel
            {
                UserName = user.UserName,
                Image = user.Image
            };

            return View(userViewModel);
        }
    }
}

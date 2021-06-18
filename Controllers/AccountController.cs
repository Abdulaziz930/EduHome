using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Areas.AdminPanel.Utils;
using EduHome.Data;
using EduHome.Models;
using EduHome.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduHome.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Login

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var existUser = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if(existUser == null)
            {
                ModelState.AddModelError("", "Email or password is invalid.");
                return View();
            }

            if(existUser.IsActive == false)
            {
                ModelState.AddModelError("", "Your account is disabled.");
                return View();
            }

            var loginResult = await _signInManager.PasswordSignInAsync(existUser, loginViewModel.Password, 
                loginViewModel.RememberMe, true);
            if (!loginResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is invalid.");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var dbUser = await _userManager.FindByNameAsync(registerViewModel.UserName);
            if (dbUser != null)
            {
                ModelState.AddModelError("Username", "There is a user with this name!");
                return View();
            }

            var newUser = new User
            {
                FullName = registerViewModel.FullName,
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                IsActive = true
            };

            var identityResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(newUser, RoleConstants.MemberRole);
            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region ResetPassword

        public IActionResult RedirectionToResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedirectionToResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var dbUser = await _userManager.FindByEmailAsync(email);

            if (dbUser == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(dbUser);

            var link = Url.Action("ResetPassword", "Account", new { dbUser.Id, token }, protocol: HttpContext.Request.Scheme);
            var message = $"<a href={link}>For Reset password click here</a>";
            await EmailUtil.SendEmailAsync(dbUser.Email, message, "ResetPassword");

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ResetPassword(string id, string token)
        {

            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (string.IsNullOrEmpty(token))
                return BadRequest();

            var dbUser = await _userManager.FindByIdAsync(id);

            if (dbUser == null)
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string token, ResetPasswordViewModel passwordViewModel)
        {

            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (string.IsNullOrEmpty(token))
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(passwordViewModel);
            }

            var dbUser = await _userManager.FindByIdAsync(id);

            if (id == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(dbUser, token, passwordViewModel.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(passwordViewModel);
            }

            return RedirectToAction("Login");
        }

        #endregion

    }
}

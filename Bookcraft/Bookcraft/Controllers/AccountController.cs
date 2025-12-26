using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bookcraft.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userHelper = userHelper;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UserRegistration(string userDetails)
        {
            if (userDetails != null)
            {
                var appUserViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userDetails);
                if (appUserViewModel != null)
                {
                    var checkEmail = await _userHelper.FindByEmailAsync(appUserViewModel.Email).ConfigureAwait(false);
                    if (checkEmail != null)
                    {
                        return Json(new { isError = true, msg = "Email Already Exists" });
                    }
                    if (appUserViewModel.Password != appUserViewModel.ConfirmPassword)
                    {
                        return Json(new { isError = true, msg = "Password and Confirm password do not match" });
                    }
                    if (appUserViewModel.Password.Length < 8)
                    {
                        return Json(new { isError = true, msg = "Password must be from 8 characters" });
                    }
                    var createUser = await _userHelper.RegisterUser(appUserViewModel).ConfigureAwait(false);
                    if (createUser)
                    {
                        return Json(new { isError = false, msg = "Registration Successful, Login to continue" });
                    }
                    return Json(new { isError = true, msg = "Unable to register" });
                }
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(string email, string password)
        {
            if (email != null && password != null)
            {
                var filterSpace = email.Replace(" ", "");
                var url = "";
                var existingUser = _userHelper.FindByEmailAsync(filterSpace).Result;
                if (existingUser != null)
                {
                    var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                    if (PasswordSignIn.Succeeded)
                    {
                        var userRole = await _userManager.GetRolesAsync(existingUser).ConfigureAwait(false);
                        if (userRole.FirstOrDefault().ToLower().Contains("admin"))
                        {
                            url = "/Admin/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                        if (userRole.FirstOrDefault().ToLower().Contains("user"))
                        {
                            url = "/User/Index";
                            return Json(new { isError = false, dashboard = url });
                        }

                    }
                }
                return Json(new { isError = true, msg = "Account does not exist, contact admin" });


            }
            return Json(new { isError = true, msg = "Username and Password Required" });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PasswordForgot(string email)
        {
            if (email != null)
            {
                var getUser = _userHelper.FindByEmailAsync(email).Result;
                if (getUser != null) 
                {
                    string linkToClick = HttpContext.Request.Scheme.ToString() + "://" +
                    HttpContext.Request.Host.ToString() + "/Account/ResetPassword?userId=" + getUser.Id;
                    var sendResetPasswordLink = _userHelper.ResetPasswordLink(email, linkToClick, getUser.FullName);
                    if (sendResetPasswordLink)
                    {
                        return Json(new { isError = false, msg = "Password reset link has been sent to your email" });
                    }
                }
                return Json(new { isError = true, msg = "User not found" });
            }
            return Json(new { isError = true, msg = "Email address needed" });
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId)
        {
            if (userId != null)
            {
                var model = new ApplicationUserViewModel
                {
                    Id = userId,
                };
                return View(model);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public JsonResult Reset(string password, string confirmPassword, string userId)
        {
            if (password != null && confirmPassword != null && userId != null)
            {
                if (password != confirmPassword)
                {
                    return Json(new { isError = true, msg = "Password and Confirm password do not match" });
                }
                var getUserById = _userHelper.FindById(userId);
                if (getUserById != null)
                {
                    var removeExistingPassword = _userManager.RemovePasswordAsync(getUserById).Result;
                    if (removeExistingPassword.Succeeded)
                    {
                        var addNewPassword = _userManager.AddPasswordAsync(getUserById, password).Result;
                        if (addNewPassword.Succeeded)
                        {
                            return Json(new { isError = false, msg = "Password Reset Successful" });
                        }
                    }
                }
                return Json(new { isError = true, msg = "User not found" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }




    }
}

using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public UserHelper(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AppDbContext context, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.Users.Where(s => s.Email == email)?.FirstOrDefaultAsync();
        }
        public ApplicationUser FindByUserName(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username).FirstOrDefault();
        }
        public async Task<ApplicationUser> FindByUserNameAsync(string username)
        {
            return await _userManager.Users.Where(s => s.UserName == username).FirstOrDefaultAsync();
        }

        //public int GetAllUsers()
        //{
        //    return _userManager.Users.Where(x => !x.Deactivated).Count();
        //}
        public string GetUserRole(string userId)
        {
            if (userId != null)
            {
                var userRole = _context.UserRoles.Where(x => x.UserId == userId).FirstOrDefault();
                if (userRole != null)
                {
                    var roles = _context.Roles.Where(x => x.Id == userRole.RoleId).FirstOrDefault();
                    if (roles != null)
                    {
                        return roles.Name;
                    }
                }
            }
            return null;
        }
        public string GetUserId(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefaultAsync().Result.Id?.ToString();
        }
        public ApplicationUser FindById(string Id)
        {
            return _userManager.Users.Where(s => s.Id == Id).FirstOrDefault();
        }
        public string GetCurrentUserId(string username)
        {
            try
            {
                if (username != null)
                {
                    return _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefault()?.Id?.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //public string GenerateStudentID()
        //{
        //    var dateConvert = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
        //    int sequence = 10;
        //    var initial = "EmusINS";
        //    var studentId = initial + dateConvert + sequence;
        //    return studentId;
        //}

        public async Task<bool> RegisterUser(ApplicationUserViewModel userDetails)
        {
            try
            {
                if (userDetails != null)
                {
                    var user = new ApplicationUser();
                    user.UserName = userDetails.Email;
                    user.Email = userDetails.Email;
                    user.FullName = userDetails.FullName;
                    user.PhoneNumber = "N/A";
                    user.DateRegistered = DateTime.Now;
                    user.Deactivated = false;
                    user.IsAdmin = false;
                    var createUser = await _userManager.CreateAsync(user, userDetails.Password).ConfigureAwait(false);
                    if (createUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User").ConfigureAwait(false);
                        return true;

                        //if (user.Email != null)
                        //{
                        //    string toEmail = user.Email;
                        //    string subject = "Application Successful";
                        //    string message = "Hello," + "<b>" + user?.FullName + ",</b> " +
                        //        "<br> Welcome to Booksphere. " +
                        //        "<br> Your book writing jouney starts here. Feel free to create exciting books. " +
                        //        "<br> Once again, welcome! " +
                        //        "<br> <br> Booksphere Team";

                        //    _emailService.SendEmail(toEmail, subject, message);
                        //    return true;
                        //}

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RegisterAdmin(ApplicationUserViewModel userDetails)
        {
            try
            {
                if (userDetails != null)
                {
                    var user = new ApplicationUser();
                    user.UserName = userDetails.Email;
                    user.Email = userDetails.Email;
                    user.FullName = userDetails.FullName;
                    user.PhoneNumber = "N/A";
                    user.DateRegistered = DateTime.Now;
                    user.Deactivated = false;
                    user.IsAdmin = false;
                    var createUser = await _userManager.CreateAsync(user, userDetails.Password).ConfigureAwait(false);
                    if (createUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin").ConfigureAwait(false);
                        return true;

                        //if (user.Email != null)
                        //{
                        //    string toEmail = user.Email;
                        //    string subject = "Application Successful";
                        //    string message = "Hello," + "<b>" + user?.FullName + ",</b> " +
                        //        "<br> Welcome to Booksphere. " +
                        //        "<br> You are now an admin at Bookspher. Your book writing jouney starts here. Feel free to create exciting books. " +
                        //        "<br> Once again, welcome! " +
                        //        "<br> <br> Booksphere Team";

                        //    _emailService.SendEmail(toEmail, subject, message);
                        //    return true;
                        //}

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ResetPasswordLink(string email, string linkToClick, string fullname)
        {
            if (email != null)
            {
                var url = linkToClick;
                string toEmail = email;
                string subject = "Password Reset Link";
                string message = "Hello," + "<b>" + fullname +",</b> " +
                   ". <br> Please, click on the button below to reset your password." +
                    "<br>" + "<a style:'border:2px; text-decoration: none;' href='" + url + "' target='_blank'>" + "<button style='color:white; background-color:#524dd3; padding:12px; border:1px solid #524dd3;'> Reset Password </button>" + "</a>" +
                    "<br/> <br/> Thank you. " +
                    "<br/> <br/> Booksphere Team";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            }

            return false;
        }
    }
}

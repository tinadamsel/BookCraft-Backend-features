using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        ApplicationUser FindById(string Id);
        ApplicationUser FindByUserName(string username);
        string GetCurrentUserId(string username);
        string GetUserId(string username);
        string GetUserRole(string userId);
        Task<bool> RegisterUser(ApplicationUserViewModel userDetails);
        bool ResetPasswordLink(string email, string linkToClick, string fullname);
    }
}

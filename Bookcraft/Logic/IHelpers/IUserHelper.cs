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
        bool CheckExistingGenreName(string name);
        bool CheckExistingStyleName(string name);
        bool CheckExistingTargetAudience(string name);
        bool CreateAudience(string name);
        bool CreateGenre(string name);
        bool CreateWritingStyle(string name);
        bool DeleteAudience(int id);
        bool DeleteGenre(int id);
        bool DeleteStyle(int id);
        Task<ApplicationUser> FindByEmailAsync(string email);
        ApplicationUser FindById(string Id);
        ApplicationUser FindByUserName(string username);
        List<TargetAudienceViewModel> GetAllAudience();
        TargetAudienceViewModel GetAudienceToEdit(int id);
        string GetCurrentUserId(string username);
        List<BookGenreViewModel> GetGenre();
        BookGenreViewModel GetGenreToEdit(int id);
        WritingStylesViewModel GetStyleToEdit(int id);
        string GetUserId(string username);
        string GetUserRole(string userId);
        List<WritingStylesViewModel> GetWritingStyles();
        Task<bool> RegisterAdmin(ApplicationUserViewModel userDetails);
        Task<bool> RegisterUser(ApplicationUserViewModel userDetails);
        bool ResetPasswordLink(string email, string linkToClick, string fullname);
        bool SaveEditedAudience(int id, string name);
        bool SaveEditedGenre(int id, string name);
        bool SaveEditedStyle(int id, string name);
    }
}

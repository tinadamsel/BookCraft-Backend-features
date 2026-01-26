using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface IBookService
    {
        Task<Book> SaveBookDetailsToDbAsync(BookViewModel bookViewModel, string userId);
        //Task<string> GenerateTextAsync(string prompt);
        Task<string> GenerateAsync(string prompt);
        Task GenerateBookChaptersAsync(int bookId);
    }
}

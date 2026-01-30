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
        Task<byte[]> ExportBookToDocxAsync(int bookId);
        byte[] ExportBookToEpub(int bookId);
        byte[] ExportBookToPdf(int bookId);
        //Task<byte[]> ExportBookToPdfAsync(int bookId);

        //Task<string> GenerateTextAsync(string prompt);
        Task<string> GenerateAsync(string prompt);
        //Task GenerateBookChaptersAsync(int bookId);
        Task<int> GenerateBookChaptersAsync(int bookId);
        Task<byte[]> GenerateBookCoverAsync(string prompt);
        Book GetBook(int bookId);
        BookViewModel GetBookWithChapters(int bookId);
        List<BookViewModel> GetUserBooks(ApplicationUser currentUser);
        Task SaveBookCover(int bookId, string coverUrl);
        Task<Book> SaveBookDetailsToDbAsync(BookViewModel bookViewModel, ApplicationUser user);
        bool SaveEditedChapter(BookChaptersViewModel chapterViewModel);
    }
}

using Core.Config;
using Core.DB;
using Core.DTO;
using Core.Models;
using Core.ViewModels;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.BooksphereEnums;
using static Core.DTO.ResponseDTO;

namespace Logic.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly BookSettings _settings;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly AppDbContext _context;

        public BookService(HttpClient httpClient, IOptions<BookSettings> settings, IGeneralConfiguration generalConfiguration, AppDbContext context)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _generalConfiguration = generalConfiguration;
            _context = context;
            _httpClient.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", _generalConfiguration.ChatGptAuthorization);
        }
        public async Task<Book> SaveBookDetailsToDbAsync(BookViewModel bookViewModel, string userId)
        {
            var book = new Book
            {
                BookAuthor = userId,
                BookStatus = BookStatus.Inprogress,
                BookTitle = bookViewModel.BookTitle,
                BookDescription = bookViewModel.BookDescription,
                GenreId = bookViewModel.GenreId,
                WritingStyleId = bookViewModel.WritingStyleId,
                TargetAudienceId = bookViewModel.TargetAudienceId,
                BookImageUpload =  bookViewModel.BookImageUpload,
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task GenerateBookChaptersAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.TargetAudience)
                .Include(b => b.WritingStyle)
                .Include(b => b.Chapters)
                .FirstAsync(b => b.Id == bookId);

            var outline = await GenerateOutlineAsync(book);

            //from here cut out the method, generate chapter differently 
            int chapterNumber = 1;
            foreach (var chapter in outline.Chapters)
            {
                var content = await GenerateChapterAsync(
                    book,
                    chapter.Title,
                    chapterNumber
                );

                _context.Chapters.Add(new Chapters
                {
                    BookId = book.Id,
                    ChapterNumber = chapterNumber,
                    ChapterTitle = chapter.Title,
                    ChapterContent = content,
                    CreatedAt = DateTime.Now,
                    Status = ChapterStatus.Generated
                });

                chapterNumber++;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<BookOutlineDto> GenerateOutlineAsync(Book book)
        {
            var prompt = $@"
                        Create a detailed book outline.

                        Book Title: {book.BookTitle}
                        Genre: {book.Genre.Name}
                        Target Audience: {book.TargetAudience.Name}
                        Writing Style: {book.WritingStyle.Name}
                        Pages: {book.PageSize}

                        Return ONLY chapter titles as a numbered list.
                        ";

            var response = await GenerateAsync(prompt);

            var chapters = response
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new OutlineChapterDto
                {
                    Title = x.Substring(x.IndexOf('.') + 1).Trim()
                })
                .ToList();

            return new BookOutlineDto { Chapters = chapters };
        }
        //return the list of outline and populate to the front, then generate the whole chapters

        public async Task<string> GenerateChapterAsync(Book book, string chapterTitle,int chapterNumber)
        {
            var prompt = $@"
                    Write Chapter {chapterNumber} of a book.
                    Book Title: {book.BookTitle}
                    Genre: {book.Genre.Name}
                    Writing Style: {book.WritingStyle.Name}
                    Target Audience: {book.TargetAudience.Name}
                    Chapter Title: {chapterTitle}
                    Write a full chapter with engaging content and also add sketch diagrams if necessary.
                    ";
                return await GenerateAsync(prompt);
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var request = new OpenAiChatRequest
            {
                model = "gpt-4.1-mini",
                messages = new List<Message>
            {
                new Message { role = "system", content = "You are a professional book author." },
                new Message { role = "user", content = prompt }
            }
            };

            var response = await _httpClient.PostAsJsonAsync(_generalConfiguration.ChatGptBaseAddress,
                request
            );
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>();
            return result!.choices.First().message.content;
            //return result.Output;
        }


    }


}

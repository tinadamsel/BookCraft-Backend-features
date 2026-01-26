using Core.Config;
using Core.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class CoverGenerationService : ICoverGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly AppDbContext _context;

        public CoverGenerationService(HttpClient httpClient, IGeneralConfiguration generalConfiguration, AppDbContext context)
        {
            _httpClient = httpClient;
            _generalConfiguration = generalConfiguration;
            _context = context;
        }

        //public async Task<string> GenerateCoverAsync(int bookId)
        //{
        //    var book = await _context.Books.FindAsync(bookId);

        //    var prompt = $"Generate a beautiful book cover for {book.BookTitle}, {book.Genre} style";

        //    return await GenerateImageAsync(prompt);

        //}
    }
}

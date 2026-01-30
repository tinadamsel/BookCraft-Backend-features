using Core.Config;
using Core.DB;
using Core.DTO;
using Core.Models;
using Core.ViewModels;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using OpenXmlPowerTools;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Core.DB.BooksphereEnums;
using static Core.DTO.ResponseDTO;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Logic.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly BookSettings _settings;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly AppDbContext _context;
        private readonly IConverter _pdfConverter;

        public BookService(HttpClient httpClient, IOptions<BookSettings> settings, IGeneralConfiguration generalConfiguration, AppDbContext context, IConverter pdfConverter)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _generalConfiguration = generalConfiguration;
            _context = context;
            _httpClient.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", _generalConfiguration.ChatGptAuthorization);
            _pdfConverter = pdfConverter;
        }
        public async Task<Book> SaveBookDetailsToDbAsync(BookViewModel bookViewModel, ApplicationUser user)
        {
            var book = new Book
            {
                BookAuthor = user.FullName,
                BookStatus = BookStatus.Inprogress,
                BookTitle = bookViewModel.BookTitle,
                BookDescription = bookViewModel.BookDescription,
                GenreId = bookViewModel.GenreId,
                WritingStyleId = bookViewModel.WritingStyleId,
                TargetAudienceId = bookViewModel.TargetAudienceId,
                BookImageUpload =  bookViewModel.BookImageUpload,
                PageSize = bookViewModel.PageSize,
                UserId = user.Id,
                DateCreated = DateTime.Now,
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<int> GenerateBookChaptersAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.TargetAudience)
                .Include(b => b.WritingStyle)
                .Include(b => b.Chapters)
                .FirstOrDefaultAsync(b => b.Id == bookId);

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
            GetTotalBookChapters(bookId);
            return bookId;
            
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

                        Return ONLY 20 chapter titles as a numbered list.
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

        public async Task<string> GenerateChapterAsync(Book book, string chapterTitle,int chapterNumber)
        {
            var prompt = $@"
                    Write Chapter {chapterNumber} of a book.
                    Book Title: {book.BookTitle}
                    Genre: {book.Genre.Name}
                    Writing Style: {book.WritingStyle.Name}
                    Target Audience: {book.TargetAudience.Name}
                    Chapter Title: {chapterTitle}
                    Write a full chapter of not more than 5000 words with engaging content and also add sketch diagrams if necessary.
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

        public int GetTotalBookChapters(int bookId)
        {
            if (bookId > 0)
            {
                var totalChapterNo = _context.Chapters.Where(a => a.BookId == bookId).Count();
                if (totalChapterNo > 0)
                {
                    UpdateBookOutline(bookId, totalChapterNo);
                }
            }
            return 0;
        }

        public int UpdateBookOutline(int bookId, int totalChapters)
        {
            var getBook = _context.Books.Where(x => x.Id == bookId).FirstOrDefault();
            if (getBook != null)
            {
                getBook.BookOutline = totalChapters;
                _context.Update(getBook);
                _context.SaveChanges();
                return getBook.Id;
            }
            return 0;
        }

        public BookViewModel GetBookWithChapters(int bookId)
        {
            try
            {
                var book = _context.Books
                .AsNoTracking()
                .Include(b => b.Genre)
                .Include(b => b.WritingStyle)
                .Include(b => b.TargetAudience)
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == bookId);

                if (book != null)
                { 
                    var chapters = _context.Chapters.AsNoTracking().Where(c => c.BookId == bookId)
                    .OrderBy(c => c.ChapterNumber)
                    .Select(c => new BookChaptersViewModel
                    {
                        Id = c.Id,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        ChapterContent = c.ChapterContent
                    })
                    .ToList();

                    var model = new BookViewModel
                    {
                        Id = book.Id,
                        BookTitle = book.BookTitle,
                        BookAuthor = book.User.FullName,
                        BookDescription = book?.BookDescription,
                        TotalChapters = book?.BookOutline, // or however you store it
                        PageSize = book.PageSize,
                        GenreName = book.Genre?.Name,
                        WritingStyleName = book.WritingStyle?.Name,
                        TargetAudienceName = book.TargetAudience?.Name,
                        DateCreated = book.DateCreated,
                        Chapters = chapters
                    };
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public bool SaveEditedChapter(BookChaptersViewModel chapterViewModel)
        {
            if (chapterViewModel != null)
            {
                var chapter = _context.Chapters.Where(a => a.Id == chapterViewModel.Id && a.BookId == chapterViewModel.BookId).FirstOrDefault();
                if (chapter != null)
                {
                    chapter.Content = chapterViewModel.ChapterContent;
                    chapter.Status = ChapterStatus.Edited;
                }
                _context.Update(chapter);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public BookDownloadDto GetBookWithChaptersToDownload(int bookId)
        {
            var book = _context.Books
                .Include(b => b.Chapters)
                .First(b => b.Id == bookId);

            return new BookDownloadDto
            {
                BookTitle = book.BookTitle,
                BookAuthor = book.BookAuthor,
                CoverImageAbsoluteUrl = $"{_generalConfiguration.BaseUrl}{book.CoverDesignUrl}",
                Chapters = book.Chapters.Select(c => new ChapterDownloadDto
                {
                    ChapterNumber = c.ChapterNumber,
                    ChapterTitle = c.ChapterTitle,
                    Content = c.Content
                }).ToList()
            };
        }

        //export pdf

        public byte[] ExportBookToPdf(int bookId)
        {
            var book = GetBookWithChaptersToDownload(bookId);

            // ✅ Resolve cover image path (local file system)
            byte[]? coverImageBytes = null;

            if (!string.IsNullOrWhiteSpace(book.CoverImageAbsoluteUrl))
            {
                // Convert absolute URL → local path
                // Example: https://site.com/book-covers/abc.png
                // → wwwroot/book-covers/abc.png
                var relativePath = book.CoverImageAbsoluteUrl
                    .Replace(_generalConfiguration.BaseUrl, "")
                    .TrimStart('/');

                var physicalPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    relativePath
                );

                if (System.IO.File.Exists(physicalPath))
                {
                    coverImageBytes = System.IO.File.ReadAllBytes(physicalPath);
                }
            }

            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        // ✅ COVER PAGE
                        if (coverImageBytes != null)
                        {
                            col.Item()
                               .AlignCenter()
                               .Image(coverImageBytes)
                               .FitArea();

                            col.Item().PaddingBottom(20);
                        }

                        col.Item()
                           .AlignCenter()
                           .Text(book.BookTitle)
                           .FontSize(24)
                           .Bold();

                        col.Item()
                           .AlignCenter()
                           .Text($"By {book.BookAuthor}")
                           .FontSize(12);

                        col.Item().PageBreak();

                        // ✅ CHAPTERS
                        foreach (var chapter in book.Chapters.OrderBy(c => c.ChapterNumber))
                        {
                            col.Item()
                               .Text($"Chapter {chapter.ChapterNumber}: {chapter.ChapterTitle}")
                               .Bold()
                               .FontSize(16);

                            col.Item()
                               .PaddingTop(5)
                               .Text(StripHtmlPreserveLines(chapter.Content))
                               .FontSize(11);

                            col.Item().PaddingBottom(20);
                        }
                    });
                });
            }).GeneratePdf();
        }

        //public byte[] ExportBookToPdf(int bookId)
        //{
        //    var book = GetBookWithChaptersToDownload(bookId);

        //    return QuestPDF.Fluent.Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Margin(30);

        //            page.Content().Column(col =>
        //            {
        //                col.Item().Text(book.BookTitle).FontSize(20).Bold();
        //                col.Item().Text($"By {book.BookAuthor}");

        //                foreach (var chapter in book.Chapters.OrderBy(c => c.ChapterNumber))
        //                {
        //                    col.Item().Text($"Chapter {chapter.ChapterNumber}: {chapter.ChapterTitle}")
        //                        .Bold()
        //                        .FontSize(16);

        //                    col.Item().PaddingTop(5)
        //                        .Text(StripHtmlPreserveLines(chapter.Content))
        //                        .FontSize(11);

        //                    col.Item().PaddingBottom(20);
        //                }
        //            });
        //        });
        //    }).GeneratePdf();
        //}

        public static string StripHtmlPreserveLines(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            html = WebUtility.HtmlDecode(html);

            html = html
                .Replace("<br>", "\n")
                .Replace("<br/>", "\n")
                .Replace("<br />", "\n")
                .Replace("</p>", "\n\n")
                .Replace("</div>", "\n");

            return Regex.Replace(html, "<.*?>", string.Empty).Trim();
        }

        //export to docx
        public async Task<byte[]> ExportBookToDocxAsync(int bookId)
        {
            var book = GetBookWithChaptersToDownload(bookId);

            using var ms = new MemoryStream();

            using (var wordDoc = WordprocessingDocument.Create(
                ms,
                WordprocessingDocumentType.Document,
                true))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var body = mainPart.Document.Body;

                // =============================
                // 1. COVER IMAGE (IF EXISTS)
                // =============================
                if (!string.IsNullOrWhiteSpace(book.CoverImageAbsoluteUrl))
                {
                    var imageAdded = AddCoverImage(mainPart, body, book.CoverImageAbsoluteUrl);

                    if (imageAdded)
                    {
                        body.Append(new Paragraph(
                            new Run(new Break { Type = BreakValues.Page })
                        ));
                    }
                }

                // =============================
                // 2. TITLE & AUTHOR
                // =============================
                body.Append(CreateParagraph(book.BookTitle, true));
                body.Append(CreateParagraph($"Author: {book.BookAuthor}", false));

                body.Append(new Paragraph(
                    new Run(new Break { Type = BreakValues.Page })
                ));

                // =============================
                // 3. CHAPTERS
                // =============================
                foreach (var chapter in book.Chapters.OrderBy(c => c.ChapterNumber))
                {
                    body.Append(CreateParagraph(
                        $"Chapter {chapter.ChapterNumber}: {chapter.ChapterTitle}",
                        true
                    ));

                    body.Append(CreateParagraph(
                        StripHtml(chapter.Content),
                        false
                    ));
                }

                mainPart.Document.Save();
                // ❌ NO Close() needed
            }

            return ms.ToArray(); // ✅ stream now contains the file
        }

        private Paragraph CreateParagraph(string text, bool isHeading)
        {
            return new Paragraph(
                new Run(
                    new RunProperties(
                        isHeading ? new Bold() : null,
                        new FontSize { Val = isHeading ? "28" : "22" }
                    ),
                    new Text(text ?? "") { Space = SpaceProcessingModeValues.Preserve }
                )
            );
        }

        private string StripHtml(string html)
        {
            return Regex.Replace(html ?? "", "<.*?>", string.Empty);
        }

        private bool AddCoverImage(MainDocumentPart mainPart,Body body, string imageUrl)
        {
            // Convert absolute URL to physical path
            var relativePath = imageUrl.Replace(_generalConfiguration.BaseUrl, "")
                                       .TrimStart('/');

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relativePath.Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (!File.Exists(physicalPath))
                return false;

            var imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

            using (var stream = File.OpenRead(physicalPath))
            {
                imagePart.FeedData(stream);
            }

            var imageId = mainPart.GetIdOfPart(imagePart);

            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = 990000L, Cy = 1400000L },
                    new DW.EffectExtent(),
                    new DW.DocProperties
                    {
                        Id = 1U,
                        Name = "Book Cover"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties
                                    {
                                        Id = 0U,
                                        Name = "Cover Image"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()
                                ),
                                new PIC.BlipFill(
                                    new A.Blip { Embed = imageId },
                                    new A.Stretch(new A.FillRectangle())
                                ),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset(),
                                        new A.Extents { Cx = 990000L, Cy = 1400000L }
                                    ),
                                    new A.PresetGeometry(
                                        new A.AdjustValueList()
                                    )
                                    { Preset = A.ShapeTypeValues.Rectangle }
                                )
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
            );

            body.Append(new Paragraph(new Run(element)));
            return true;
        }

        public byte[] ExportBookToEpub(int bookId)
        {
            var book = GetBookWithChaptersToDownload(bookId);

            using var ms = new MemoryStream();
            using var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);

            int index = 1;

            // =============================
            // 1. ADD COVER PAGE (IF EXISTS)
            // =============================
            if (!string.IsNullOrWhiteSpace(book.CoverImageAbsoluteUrl))
            {
                // Add cover HTML
                var coverEntry = archive.CreateEntry("cover.html");
                using (var writer = new StreamWriter(coverEntry.Open()))
                {
                    writer.Write($@"
                <html>
                    <body style='text-align:center;'>
                        <img src='cover.png' style='max-width:100%; height:auto;' />
                        <h1>{book.BookTitle}</h1>
                        <p><strong>Author:</strong> {book.BookAuthor}</p>
                    </body>
                </html>
            ");
                }

                // Add cover image file
                var coverImageEntry = archive.CreateEntry("cover.png");
                using var imageStream = coverImageEntry.Open();

                // Convert absolute URL → local file path
                var localPath = ConvertCoverUrlToLocalPath(book.CoverImageAbsoluteUrl);
                var imageBytes = File.ReadAllBytes(localPath);
                imageStream.Write(imageBytes, 0, imageBytes.Length);
            }

            // =============================
            // 2. ADD CHAPTERS
            // =============================
            foreach (var chapter in book.Chapters.OrderBy(c => c.ChapterNumber))
            {
                var entry = archive.CreateEntry($"chapter{index}.html");

                using var writer = new StreamWriter(entry.Open());
                writer.Write($@"
                    <html>
                        <body>
                            <h2>Chapter {chapter.ChapterNumber}: {chapter.ChapterTitle}</h2>
                            {chapter.Content}
                        </body>
                    </html>
                ");

                index++;
            }

            return ms.ToArray();
        }

        private string ConvertCoverUrlToLocalPath(string coverUrl)
        {
            // Remove base URL if present
            var relativePath = coverUrl.Replace(_generalConfiguration.BaseUrl, "")
                                       .TrimStart('/');

            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relativePath.Replace("/", Path.DirectorySeparatorChar.ToString())
            );
        }

       

        private string BuildBookHtml(BookDownloadDto book)
        {
            var sb = new StringBuilder();

            var coverImageHtml = !string.IsNullOrWhiteSpace(book.CoverImageAbsoluteUrl)
                ? $"<img src='{book.CoverImageAbsoluteUrl}' />"
                : string.Empty;

            sb.Append($@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    body {{
                        font-family: Arial, Helvetica, sans-serif;
                        line-height: 1.6;
                    }}

                    .cover-page {{
                        text-align: center;
                        page-break-after: always;
                    }}

                    .cover-page img {{
                        max-width: 100%;
                        height: auto;
                        margin-bottom: 20px;
                    }}

                    h1 {{
                        text-align: center;
                        margin-top: 40px;
                    }}

                    .chapter {{
                        page-break-before: always;
                    }}
                </style>
                </head>
                <body>

            <!-- COVER PAGE -->
            <div class='cover-page'>
                {coverImageHtml}
                <h1>{book.BookTitle}</h1>
                <p><strong>Author:</strong> {book.BookAuthor}</p>
            </div>
            ");

            // Chapters
            foreach (var chapter in book.Chapters.OrderBy(c => c.ChapterNumber))
            {
                sb.Append($@"
                <div class='chapter'>
                    <h2>Chapter {chapter.ChapterNumber}: {chapter.ChapterTitle}</h2>
                    <p>{chapter.Content.Replace("\n", "<br/>")}</p>
                </div>
                ");
            }

            sb.Append("</body></html>");

            return sb.ToString();
        }

        public Book GetBook(int bookId)
        {
            if (bookId > 0)
            {
                var getBook = _context.Books.Where(x => x.Id == bookId).FirstOrDefault();
                if (getBook != null)
                {
                    return getBook;
                }
            }
            return null;
        }

        public async Task<byte[]> GenerateBookCoverAsync(string prompt)
        {
            var result = await GenerateImageAsync(prompt, width: 1024, height: 1536);
            return result;
        }

        public async Task<byte[]> GenerateImageAsync(string prompt,int width = 1024, int height = 1536)
        {
            var requestBody = new
            {
                model = "gpt-image-1",
                prompt = prompt,
                size = $"{width}x{height}"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _generalConfiguration.ChatGptImageAddress);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _generalConfiguration.ChatGptAuthorization);

            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Image generation failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var imageResponse = JsonSerializer.Deserialize<ImageGenerationResponse>(json);

            var base64Image = imageResponse!.data.First().b64_json;

            return Convert.FromBase64String(base64Image);
        }

        public async Task SaveBookCover(int bookId, string coverUrl)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                throw new Exception("Book not found");

            book.CoverDesignUrl = coverUrl;
            book.BookStatus = BookStatus.Completed;

            await _context.SaveChangesAsync();
        }

        public List<BookViewModel> GetUserBooks(ApplicationUser currentUser)
        {
            var bookViewModel = new List<BookViewModel>();
            bookViewModel = _context.Books.Where(x => x.Id > 0 && x.UserId == currentUser.Id)
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    BookAuthor = x.BookAuthor,
                    BookTitle = x.BookTitle,
                    CoverDesignUrl = x.CoverDesignUrl,
                    BookDescription = x.BookDescription,
                    BookOutline = x.BookOutline,
                    TotalChapters = x.BookOutline,
                    PageSize = x.PageSize,
                    DateCreated = x.DateCreated,
                }).ToList();
            return bookViewModel;
        }


    }
}

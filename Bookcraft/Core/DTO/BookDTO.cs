using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    class BookDTO
    {
        public string Title { get; set; }
        //public string? Genre { get; set; }
        //public string? WritingStyle { get; set; }
        //public string? TargetAudience { get; set; }
        //public int? ChapterCount { get; set; }
        //public int? PageSize { get; set; }
        //public int? WordsPerChapter { get; set; }
        //public string? AdditionalInstructions { get; set; }
        public Book? BookDetails { get; set; }
        public string BookTitle { get; set; }
        public int PageSize { get; set; }
        public int GenreId { get; set; }
        public int WritingStyleId { get; set; }
        public int TargetAudienceId { get; set; }
    }

    public class BookOutlineDto
    {
        public List<OutlineChapterDto> Chapters { get; set; } = new();
    }

    public class OutlineChapterDto
    {
        public string Title { get; set; }
    }

    public class BookDownloadDto
    {
        public int BookId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string BookAuthor { get; set; } = string.Empty;

        /// <summary>
        /// Absolute URL or file path used by PDF/DOCX/EPUB exporters
        /// </summary>
        public string CoverImageAbsoluteUrl { get; set; } = string.Empty;

        /// <summary>
        /// Relative URL (useful for web preview)
        /// </summary>
        public string CoverImageUrl { get; set; } = string.Empty;

        public int TotalChapters { get; set; }

        public int EstimatedPages { get; set; }

        public DateTime GeneratedOn { get; set; }

        public List<ChapterDownloadDto> Chapters { get; set; } = new();
    }

    public class ChapterDownloadDto
    {
        public int ChapterId { get; set; }

        public int? ChapterNumber { get; set; }

        public string? ChapterTitle { get; set; } = string.Empty;

        /// <summary>
        /// Final edited content used for export
        /// </summary>
        public string? Content { get; set; } = string.Empty;
    }


}

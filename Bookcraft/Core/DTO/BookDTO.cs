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
}

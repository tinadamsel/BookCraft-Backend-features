using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.BooksphereEnums;

namespace Core.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string? BookTitle { get; set; }
        public string? BookOutline { get; set; }
        public string BookDescription { get; set; }
        public string? BookImageUpload { get; set; }
        public string? ExtractedText { get; set; }
        public List<Chapters> Chapters { get; set; }
        public string? CoverDesignUrl { get; set; }
        public string? BookAuthor { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
       
        public int GenreId { get; set; }
        public virtual BookGenre? Genre { get; set; }
       
        public int TargetAudienceId { get; set; }
        public virtual TargetAudience? TargetAudience { get; set; }
       
        public int WritingStyleId { get; set; }
        public virtual WritingStyle? WritingStyle { get; set; }
        public BookStatus BookStatus { get; set; }
        public ICollection<Chapters> BookChapters { get; set; }

    }
}

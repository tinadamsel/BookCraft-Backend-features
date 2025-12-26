using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.BooksphereEnums;

namespace Core.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string BookOutline { get; set; }
        public List<Chapters> Chapters { get; set; }
        public string CoverDesignUrl { get; set; }
        public string BookAuthor { get; set; }
        
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        [ForeignKey("GenreId")]
        public int GenreId { get; set; }
        public virtual BookGenre? Genre { get; set; }

        [ForeignKey("TargetAudienceId")]
        public int TargetAudienceId { get; set; }
        public virtual TargetAudience? TargetAudience { get; set; }

        [ForeignKey("WritingStyleId")]
        public int WritingStyleId { get; set; }
        public virtual WritingStyle? WritingStyle { get; set; }
        public BookStatus BookStatus { get; set; }

    }
}

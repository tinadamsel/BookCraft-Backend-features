using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.BooksphereEnums;

namespace Core.Models
{
    public class Chapters
    {
        public int Id { get; set; }
        public int? ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public string? ChapterContent { get; set; }
        public ChapterStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("BookId")]
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public string? Content { get; set; }

    }


}

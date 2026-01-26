using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DB
{
    public class BooksphereEnums
    {
        public enum BookStatus
        {
            [Description("For Inprogress")]
            Inprogress = 1,
            [Description("For Completed")]
            Completed = 2,
        }

        public enum ChapterStatus
        {
            Draft,
            Generated,
            Edited,
            Final
        }
    }
}

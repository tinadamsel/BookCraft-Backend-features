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
            [Description("For Pending")]
            Pending = 1,
            [Description("For Inprogress")]
            Inprogress = 2,
            [Description("For Completed")]
            Completed = 3,
        }
    }
}

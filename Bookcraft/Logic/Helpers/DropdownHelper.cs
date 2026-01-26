using Core.DB;
using Core.Models;
using Logic.IHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class DropdownHelper : IDropdownHelper
    {
        private readonly AppDbContext _context;
        public DropdownHelper(AppDbContext context)
        {
            _context = context;
        }

        public List<BookGenre> DropdownOfGenres()
        {
            try
            {
                var common = new BookGenre()
                {
                    Id = 0,
                    Name = "-- Select Genre --"
                };
                var listOfGenres = _context.BookGenres.Where(x => x.Id > 0 && x.IsActive).ToList();
                var drp = listOfGenres.Select(x => new BookGenre
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                drp.Insert(0, common);
                return drp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public List<TargetAudience> DropdownOfAudience()
        {
            try
            {
                var common = new TargetAudience()
                {
                    Id = 0,
                    Name = "-- Select Target Audience --"
                };
                var listOfTargetAudience = _context.TargetAudiences.Where(x => x.Id > 0 && x.IsActive).ToList();
                var drp = listOfTargetAudience.Select(x => new TargetAudience
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                drp.Insert(0, common);
                return drp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public List<WritingStyle> DropdownOfWritingStyles()
        {
            try
            {
                var common = new WritingStyle()
                {
                    Id = 0,
                    Name = "-- Select Writing Style --"
                };
                var listOfStyles = _context.WritingStyles.Where(x => x.Id > 0 && x.IsActive).ToList();
                var drp = listOfStyles.Select(x => new WritingStyle
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                drp.Insert(0, common);
                return drp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public class DropDown
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
        }
    }
}

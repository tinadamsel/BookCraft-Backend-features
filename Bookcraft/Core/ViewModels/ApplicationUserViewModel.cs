using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phonenumber { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool Deactivated { get; set; }
        public bool IsAdmin { get; set; }
        public int TotalBooksCreated { get; set; }
        public int BookCoverCreated { get; set; }
        public string? RefLink { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool Deactivated { get; set; }
        public bool IsAdmin { get; set; }
       
        //public string? RefLink { get; set; }
    }

}

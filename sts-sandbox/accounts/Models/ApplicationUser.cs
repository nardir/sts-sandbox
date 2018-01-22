using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace accounts.Models
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
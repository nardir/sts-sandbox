﻿using accounts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace accounts.Data
{
    public static class ApplicationUserSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            users.Add(new ApplicationUser()
            {
                Email = "nardir@axerrio.com",
                UserName = "nardir@axerrio.com",
                FirstName = "Nardi",
                LastName = "Rens",
            });

            users.Add(new ApplicationUser()
            {
                Email = "a.vangeel@ziggo.nl",
                UserName = "a.vangeel@ziggo.nl",
                FirstName = "Annemarie",
                LastName = "Geel",
            });

            foreach (var user in users)
            {
                if (!userManager.Users.Any(u => u.Email == user.Email))
                {
                    var result = await userManager.CreateAsync(user, user.LastName.ToLower());
                }
            }
        }
    }
}

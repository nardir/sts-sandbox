using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.Identity.Accounts.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Axerrio.Identity.API.Services
{
    public class LoginService : ILoginService
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;

        public LoginService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<bool> ValidateCredentialsAsync(ApplicationUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<ApplicationUser> FindByUserNameAsync(string userName)
        {
            return _userManager.FindByEmailAsync(userName);
        }

        public Task SignInAsync(ApplicationUser user, AuthenticationProperties authenticationProperties)
        {
            return _signInManager.SignInAsync(user, authenticationProperties);
        }
    }
}

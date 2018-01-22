using Axerrio.Identity.Accounts.Models;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.Identity.API.Services
{
    public interface ILoginService
    {
        Task<bool> ValidateCredentialsAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task SignInAsync(ApplicationUser user, AuthenticationProperties authenticationProperties);
    }
}
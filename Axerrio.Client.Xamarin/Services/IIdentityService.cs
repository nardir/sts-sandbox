using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Axerrio.Client.Xamarin.Services
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
    }
}

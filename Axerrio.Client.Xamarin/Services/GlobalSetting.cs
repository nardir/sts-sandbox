using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.Client.Xamarin.Services
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://localhost";

        private string _baseEndpoint;
        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";
            BaseEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance
        {
            get { return _instance; }
        }

        public string BaseEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
                UpdateEndpoint(_baseEndpoint);
            }
        }

        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string IdentityEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        public string Api1ServiceEndpoint { get; set; }

        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterWebsite = $"{baseEndpoint}:5000/Account/Register";
            IdentityEndpoint = $"{baseEndpoint}:5000/connect/authorize";
            UserInfoEndpoint = $"{baseEndpoint}:5000/connect/userinfo";
            TokenEndpoint = $"{baseEndpoint}:5000/connect/token";
            LogoutEndpoint = $"{baseEndpoint}:5000/connect/endsession";
            IdentityCallback = $"{baseEndpoint}:5000/xamarincallback";
            LogoutCallback = $"{baseEndpoint}:5000/Account/Redirecting";

            Api1ServiceEndpoint = $"{baseEndpoint}:5001";
        }
    }
}

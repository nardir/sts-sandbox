using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.Client.Xamarin.Services
{
    public class ServiceAuthenticationException : Exception
    {
        public string Content { get; }

        public ServiceAuthenticationException()
        {
        }

        public ServiceAuthenticationException(string content)
        {
            Content = content;
        }
    }
}

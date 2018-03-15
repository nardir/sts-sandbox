﻿using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ro.client
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            //var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            //if (disco.IsError)
            //{
            //    Console.WriteLine(disco.Error);
            //    return;
            //}

            // request token
            //var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            //var tokenClient = new TokenClient("http://localhost:5000/connect/token", "ro.client", "secret");
            var tokenClient = new TokenClient("https://localhost:50000/connect/token", "ro.client", "secret");

            //https://github.com/IdentityServer/IdentityServer4/issues/322
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("nardir@axerrio.com", "wrong", "api1");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("nardir@axerrio.com", "rens", "api1");
            


            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadLine();
        }
    }
}

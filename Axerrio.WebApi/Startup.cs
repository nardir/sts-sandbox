using Microsoft.Owin;
using Owin;
using System.Web.Http;
using IdentityServer3.AccessTokenValidation;

[assembly: OwinStartup(typeof(Axerrio.WebApi.Startup))]
namespace Axerrio.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //accept access tokens from identityserver and require a scope of 'api2'
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                //https://andrewlock.net/debugging-jwt-validation-problems-between-an-owin-app-and-identityserver4-2/
                //Authority = "null",
                Authority = "http://localhost:5000",
                //Authority = "https://localhost:50000",
                //IssuerName = "https://localhost:50000",
                //IssuerName = "http://localhost:5000",
                //IssuerName = "null",

                //ValidationMode = ValidationMode.ValidationEndpoint,
                ValidationMode = ValidationMode.Local,

                RequiredScopes = new[] { "api2" }
            });

            // configure web api
            var config = new HttpConfiguration();
            //config.MapHttpAttributeRoutes();

            // require authentication for all controllers
            //config.Filters.Add(new AuthorizeAttribute());

            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }
    }
}
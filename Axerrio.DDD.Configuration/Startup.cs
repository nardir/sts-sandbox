using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Configuration.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Axerrio.DDD.Configuration
{
    //https://github.com/thinkabouthub/Configuration.EntityFramework/wiki
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var setting1 = new ConfigurationSetting(new Model.ConfigurationSection("ConnectionStrings"), "connectionstring");
            setting1.SetValue("test", typeof(string));
            var value1 = setting1.GetValue<string>();

            

            var setting2 = new ConfigurationSetting(new Model.ConfigurationSection("Setting"), "setting1");
            setting2.SetValue(setting1, setting1.GetType());
            var value2 = setting2.GetValue(setting1.GetType());

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

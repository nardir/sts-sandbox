using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Configuration.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Axerrio.DDD.Configuration
{
    //https://github.com/thinkabouthub/Configuration.EntityFramework/wiki
    //https://msdn.microsoft.com/en-us/magazine/mt814420
    //https://github.com/aspnet/Configuration/tree/dev/src/Config.Json
    //https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options#reload-configuration-data-with-ioptionssnapshot
    //https://github.com/aspnet/Options/blob/dev/src/Microsoft.Extensions.Options.ConfigurationExtensions/OptionsConfigurationServiceCollectionExtensions.cs

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;


            //var data = Configuration.AsEnumerable();
            ////var data2 = Configuration.GetChildren();
            ////var env1 = Configuration.GetValue<string>("ENVIRONMENT");
            ////var env2 = Configuration.GetValue<string>("NR_TEST");
            ////var logging = Configuration.GetSection("Logging");
            ////var loggingValues = logging.AsEnumerable();

            ////var setting1 = new ConfigurationSetting(new Model.ConfigurationSection("ConnectionStrings"), "connectionstring");
            ////setting1.SetValue("test", typeof(string));
            ////setting1.SetValue("test");
            //var setting1 = ConfigurationSetting.Create(new Model.ConfigurationSection("ConnectionStrings"), "connectionstring", "test");
            //var value1 = setting1.GetValue<string>();

            ////var setting2 = new ConfigurationSetting(new Model.ConfigurationSection("Setting"), "setting1");
            ////setting2.SetValue(setting1, setting1.GetType());
            ////setting2.SetValue(setting1);
            //var setting2 = ConfigurationSetting.Create(new Model.ConfigurationSection("Setting"), "setting1", setting1);
            //var value2 = setting2.GetValue(setting1.GetType());

            //var options = new DbContextOptionsBuilder<ConfigurationContext>()
            //    .UseInMemoryDatabase("Config")
            //    .Options;

            //var input = JsonConvert.SerializeObject(options);
            //var data = JsonConfigurationParser.Parse(input, "ConfigContext", "Options");
        }

        public IConfiguration Configuration { get; }
        public IConfigurationBuilder Builder { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];

            services.AddDbContext<SettingDbContext>(options => 
            {
                //options.UseInMemoryDatabase("Settings");
                options.UseSqlServer(connectionString);
            });

            services.AddTransient<ISettingService, EFSettingService>();

            services.AddOptions();

            ////services.Configure<DbContextOptions<ConfigurationContext>>(Configuration.GetSection("ConfigContext:Options"));
            ////var optionsConfig = Configuration.GetSection("Test:Options");
            var optionsConfig = Configuration.GetSection(nameof(TestOptions));

            //var options4 = Configuration.GetValue<TestOptions>(nameof(TestOptions), new TestOptions { Description = "Default", Id = 1000 });
            //options4 = Configuration.Get<TestOptions>();
            var options4 = Configuration.Get(nameof(TestOptions), new TestOptions { Description = "Default", Id = 1000 });
            var options5 = Configuration.Get("nardi", new TestOptions { Description = "Default", Id = 1000 });
            //var options6 = Configuration.Get(nameof(TestOptions), () => new TestOptions { Description = "Default", Id = 1000 });

            //services.Configure<TestOptions>(testOptions => 
            //{
            //    testOptions.Id = 999;
            //    testOptions.Description = "Default description";
            //    testOptions.Names = new string[] { };
            //});
            //services.Configure<TestOptions>(optionsConfig);

            //services.Configure<TestOptions>(Configuration, nameof(TestOptions));
            //services.Configure<TestOptions>(key: nameof(TestOptions), configureOptions: testOptions => 
            //{
            //    testOptions.Id = 999;
            //    testOptions.Description = "Default description";
            //    testOptions.Names = new string[] { };
            //});
            services.Configure<TestOptions>(Configuration, nameof(TestOptions), testOptions =>
            {
                testOptions.Id = 999;
                testOptions.Description = "Default description";
                testOptions.Names = new string[] { };
            });
            //services.Configure<TestOptions>(Configuration, "notexists", testOptions =>
            //{
            //    testOptions.Id = 999;
            //    testOptions.Description = "Default description";
            //    testOptions.Names = new string[] { };
            //});

            var testoptions2 = new TestOptions
            {
                Id = 200,
                //Description = "Description test options2",
                Description = null,
                Names = null
                //Names = new string[] { "test", "nardi" }
            };

            var jsontestoptions2 = JsonConvert.SerializeObject(testoptions2);

            var cd = JsonConfigurationParser.Parse(nameof(testoptions2), jsontestoptions2);

            //cd["testoptions2:Names"] = "null";
            //cd["testoptions2:Description"] = "null";

            //string s1 = "Dit is { een [ string ] met resvered }";
            //var jsons1 = JsonConvert.SerializeObject(s1);

            //var cd = new Dictionary<string, string>();
            //cd.Add(nameof(s1), null);
            //cd.Add("i1", "100");
            //cd.Add("i2", "");

            var cb = new ConfigurationBuilder()
                .AddInMemoryCollection(cd);

            var c = cb.Build();

            //var i1 = c.GetValue<int?>("i1");
            //var i2 = c.GetValue<int?>("i2");

            services.Configure<TestOptions>(configuration: c, name: nameof(testoptions2), key: nameof(testoptions2));

            var p = services.BuildServiceProvider();
            var oa = p.GetService<IOptionsSnapshot<TestOptions>>();
            var testoptionsr = oa.Get(nameof(testoptions2));

            var data = optionsConfig.AsEnumerable();

            var options1 = optionsConfig.Get<TestOptions>();

            //https://stackoverflow.com/questions/15449800/create-object-instance-of-a-class-having-its-name-in-string-variable
            var setting1 = Setting.Create(nameof(options1), options1);
            var typeName = setting1.ValueType;
            Type setting1Type = Type.GetType(typeName);
            var to1 = Activator.CreateInstance(setting1Type);
            var to2 = setting1.GetValue(setting1Type);

            var t03 = setting1.GetValue();

            TestOptions options2 = new TestOptions();
            optionsConfig.Bind(options2);

            var provider = services.BuildServiceProvider();
            var optionsAccessor = provider.GetService<IOptionsSnapshot<TestOptions>>();
            var options3 = optionsAccessor.Value;

            services.AddSingleton<IConfigurationRoot>((IConfigurationRoot)Configuration); //Nodig voor Reload

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

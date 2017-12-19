using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Configuration.Settings;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Axerrio.DDD.Configuration.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IConfigurationRoot _configuration;

        //public ValuesController(IOptions<DbContextOptions<ConfigurationContext>> optionsAccessor)
        public ValuesController(ISettingService settingService, IOptionsSnapshot<TestOptions> optionsAccessor, IConfigurationRoot configuration)
        {
            var options = optionsAccessor.Value;

            _settingService = EnsureArg.IsNotNull(settingService, nameof(settingService));

            _configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("testoptions")]
        public async Task<IActionResult> UpdateTestOptions()
        {
            //if (!ModelState.IsValid)
            //    return BadRequest();

            var testOptions = new TestOptions()
            {
                Id = 789,
                Description = "Adjusted Test options",
                Names = new string[] { "Piet2", "Jan2", "Klaas2" }
            };

            var setting = await _settingService.UpdateOrAddAsync(nameof(TestOptions), testOptions);

            _configuration.Reload();

            return Ok(setting);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value, [FromServices] IOptionsSnapshot<TestOptions> testOptionsAccessor)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

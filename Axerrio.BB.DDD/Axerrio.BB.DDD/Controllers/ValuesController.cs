using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Axerrio.BB.DDD.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        

        public ValuesController(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory)
        {
            var storeAndForward = eventBusPublishOnlyFactory.Create<StoreAndForwardEventBus>();

            var publishOnly = eventBusPublishOnlyFactory.Create<RabbitMQEventBus>();
            var publishOnly2 = eventBusPublishOnlyFactory.Create<IEventBus>();
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
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

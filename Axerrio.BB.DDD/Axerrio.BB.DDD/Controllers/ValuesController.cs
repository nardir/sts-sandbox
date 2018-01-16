using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        

        public ValuesController(IEventBusPublishOnlyFactory eventBusPublishOnlyFactory
            , IIntegrationEventsQueueService integrationEventsQueueService
            , OrderingDbContext orderingDbContext
            , IIntegrationEventsForwarderService integrationEventsForwarderService)
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    var pm = new PaymentMethod(i, $"VISA-{i}", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            //    var ie = new PaymentMethodCreatedIntegrationEvent(pm);

            //    var eqi = new IntegrationEventsQueueItem(ie);

            //    integrationEventsQueueService.EnqueueEventAsync(eqi);
            //}

            //orderingDbContext.SaveChanges();

            //var items1 = integrationEventsQueueService.DequeueEventsAsync(Guid.NewGuid()).Result;
            //var items2 = integrationEventsQueueService.DequeueEventsAsync(Guid.NewGuid()).Result;

            //for (int i = 0; i < 10; i++)
            //{
            //    var pm = new PaymentMethod(i, $"VISA-{i}", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            //    var ie = new PaymentMethodCreatedIntegrationEvent(pm);

            //    var eqi = new IntegrationEventsQueueItem(ie);

            //    integrationEventsQueueService.EnqueueEventAsync(eqi);
            //}

            //orderingDbContext.SaveChanges();

            //var storeAndForward = eventBusPublishOnlyFactory.Create<StoreAndForwardEventBus>();

            //var publishOnly = eventBusPublishOnlyFactory.Create<RabbitMQEventBus>();
            //var publishOnly2 = eventBusPublishOnlyFactory.Create<IEventBus>();

            //try
            //{
            //    integrationEventsForwarderService.ForwardAsync().GetAwaiter().GetResult();
            //}
            //catch (Exception ex)
            //{

            //}
        }

        [HttpGet("publish/{id}")]
        public async Task<IActionResult> Publish(int id, [FromServices] IIntegrationEventsService integrationEventsService, [FromServices] OrderingDbContext orderingDbContext)
        {
            var pm = new PaymentMethod(id, $"VISA-{id}", "1234-4567-9999-1111", "123", "Piet", DateTime.UtcNow.AddYears(1));
            var ie = new PaymentMethodCreatedIntegrationEvent(pm);

            await integrationEventsService.PublishAsync(ie);

            await orderingDbContext.SaveChangesAsync(); //UoW 

            return Ok();
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

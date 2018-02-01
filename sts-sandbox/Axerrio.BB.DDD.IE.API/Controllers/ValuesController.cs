﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.BB.DDD.IE.API.Application;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;

namespace Axerrio.BB.DDD.IE.API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IIntegrationEventsService _integrationEventsService;
        private readonly OrderingDbContext _context;
        private readonly IEventBusPublisher _eventBusPublisher;
        private readonly IEventBusConsumer _eventBusConsumer;

        public ValuesController(IIntegrationEventsService integrationEventsService
            , OrderingDbContext context
            , IEventBusPublisher eventBusPublisher
            , IEventBusConsumer eventBusConsumer)
        {
            _integrationEventsService = EnsureArg.IsNotNull(integrationEventsService, nameof(integrationEventsService));
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _eventBusPublisher = EnsureArg.IsNotNull(eventBusPublisher, nameof(eventBusPublisher));
            _eventBusConsumer = EnsureArg.IsNotNull(eventBusConsumer, nameof(eventBusConsumer));
        }

        [HttpGet("createorder1/{id:int}")]
        public async Task<IActionResult> CreateOrder1(int id)
        {
            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent { OrderNumber = id.ToString(), CustomerCode = id.ToString() };

            await _integrationEventsService.PublishAsync(orderCreatedIntegrationEvent);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("publish1/{id:int}")]
        public async Task<IActionResult> Publish1(int id)
        {
            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent { OrderNumber = id.ToString(), CustomerCode = id.ToString() };

            await _eventBusPublisher.PublishAsync(orderCreatedIntegrationEvent);

            return Ok();
        }

        [HttpGet("startconsume")]
        public async Task<IActionResult> StartConsume()
        {
            await _eventBusConsumer.StartConsumerAsync();

            return Ok();
        }

        [HttpGet("stopconsume")]
        public async Task<IActionResult> StopConsume()
        {
            await _eventBusConsumer.StopConsumerAsync();

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

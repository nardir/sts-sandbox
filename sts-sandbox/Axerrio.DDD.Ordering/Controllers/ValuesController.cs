﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Axerrio.DDD.Ordering.Services;
using Axerrio.DDD.Ordering.Model;
using Newtonsoft.Json;

namespace Axerrio.DDD.Ordering.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IPaymentService _paymentService;

        public ValuesController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            //var paymentMethod = new PaymentMethod(3, "alias3", "cardnumber3", "securitynumber3", "cardholder3", DateTime.Now.AddYears(1));

            //await _paymentService.CreatePaymentMethodAsync(paymentMethod);

            //var order = new Order("Nardi Rens", OrderStatus.Submitted);
            var order = new Order("Nardi Rens", OrderStatus.Submitted, 100);
            var status = order.OrderStatus;
            var json = JsonConvert.SerializeObject(order);
            var order2 = JsonConvert.DeserializeObject<Order>(json);
            var status2 = order2.OrderStatus;

            var sameStatus = (order.OrderStatus == order2.OrderStatus);
            var sameOrder = (order == order2);

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

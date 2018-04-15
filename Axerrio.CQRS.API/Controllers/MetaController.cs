using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Axerrio.CQRS.API.Application.Query;

namespace Axerrio.CQRS.API.Controllers
{
    public class MetaController: Controller
    {
        
        [HttpGet("metadata/customer")]
        public IActionResult GetCustomerMetaData()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            // types with no defined ID have their type name as the ID
            generator.SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName;

            Type custType = typeof(Customer);

            JSchema schema = generator.Generate(custType);
            schema.Title = custType.Name;
            schema.Description = "Customers";

            JSchema links = new JSchema
            {
                Type = JSchemaType.Object,
                Title = "links",
                Properties = {
                    { "uri", new JSchema { Type = JSchemaType.String } }
                }
            };

            schema.AdditionalItems = links;

            //https://blog.apisyouwonthate.com/getting-started-with-json-hyper-schema-184775b91f
            //schema.Properties.Add("uri", new JSchema { Type = JSchemaType.String, Format = "customers/{id}", Default = "customer/{id}" });

            return Ok(schema);
        }
    }
}
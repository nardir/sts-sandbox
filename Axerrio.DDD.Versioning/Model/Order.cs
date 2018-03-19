using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Axerrio.DDD.Versioning.Startup;



namespace Axerrio.DDD.Versioning.Model
{
    public class Order
    {
        public Order(int id, string number, string article, int articleKey)
        {
            OrderId = id;
            OrderNumber = number;
            SalesArticle = new Article() { Name = article, ArticleKey = articleKey };
        }
    
        public int OrderId { get; set; }

        [StringLength(25)]
        public string OrderNumber { get; set; }
        public Article SalesArticle { get; set; }

        [Range(0, 100)]
        public int Amount { get; set; }
    }
    public class AxeArticleFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {

        }
    }
    [SwaggerSchemaFilter(typeof(AxeArticleFilter))]
    public class Article
    {
        public string Name { get; set; }

        [AxeDomainType("www.jos.nl")]
        public int ArticleKey { get; set; }
    }

    public class A : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class AxeDomainTypeAttribute : Attribute
    {
        public string Url { get; set; }
        public AxeDomainTypeAttribute(string url)
        {
            Url = url;
        }
    }
}

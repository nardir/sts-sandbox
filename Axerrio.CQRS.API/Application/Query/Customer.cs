using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    //https://stackoverflow.com/questions/2656189/how-do-i-read-an-attribute-on-a-class-at-runtime
    public class Customer
    {
        //[BsonIgnoreIfDefault]
        //[BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public decimal? CreditLimit { get; set; }
        public DateTime AccountOpenedDate { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerCategoryID { get; set; }
        public CustomerCategory CustomerCategory { get; set; }
    }

    public class CustomerCategory
    {
        public int CustomerCategoryID { get; set; }
        public string CustomerCategoryName { get; set; }
    }
}
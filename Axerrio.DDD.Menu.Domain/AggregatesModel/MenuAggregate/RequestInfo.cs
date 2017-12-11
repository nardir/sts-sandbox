using Axerrio.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate
{
    public class RequestInfo : ValueObject<RequestInfo>
    {
        public string Requester { get; set; }
        public DateTime Date { get; set; }
               
        public RequestInfo(string requester, DateTime date)
        {
            Requester = requester;
            Date = date;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Requester;
            yield return Date;
        }
    }
}

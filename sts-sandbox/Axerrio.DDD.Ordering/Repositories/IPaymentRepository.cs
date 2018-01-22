using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Repositories
{
    public interface IPaymentRepository: IRepository<PaymentMethod>
    {
        void Add(PaymentMethod paymentMethod);
    }
}

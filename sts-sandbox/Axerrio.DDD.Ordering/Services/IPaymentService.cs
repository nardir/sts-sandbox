using Axerrio.DDD.Ordering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Services
{
    public interface IPaymentService
    {
        Task CreatePaymentMethodAsync(PaymentMethod paymentMethod);
    }
}

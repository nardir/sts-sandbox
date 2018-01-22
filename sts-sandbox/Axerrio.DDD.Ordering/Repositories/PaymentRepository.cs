using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Model;
using Axerrio.DDD.Ordering.Data;

namespace Axerrio.DDD.Ordering.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OrderingContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public PaymentRepository(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(PaymentMethod paymentMethod)
        {
            _context.Paymentmethods.Add(paymentMethod);
        }
    }
}

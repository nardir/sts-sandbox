using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Ordering.Model;

namespace Axerrio.DDD.Ordering.Services
{
    public class PaymentService: IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventLogRepository _integrationEventLogRepository;

        public PaymentService(IPaymentRepository paymentRepository, IIntegrationEventLogRepository integrationEventLogRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _integrationEventLogRepository = integrationEventLogRepository ?? throw new ArgumentNullException(nameof(integrationEventLogRepository));
        }

        public Task CreatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _paymentRepository.Add(paymentMethod);

            var @event = new PaymentMethodCreatedIntegrationEvent();

            @event.PaymentMethod = paymentMethod;

            _integrationEventLogRepository.Add(@event);

            return _paymentRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}

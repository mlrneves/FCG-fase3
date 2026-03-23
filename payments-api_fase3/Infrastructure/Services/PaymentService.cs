using Core.Entity;
using Core.Events;
using Core.Input;
using Core.Repository;
using Core.Services;
using Infrastructure.CrossCutting.Correlation;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IIntegrationEventPublisher integrationEventPublisher,
            ICorrelationIdGenerator correlationIdGenerator)
        {
            _paymentRepository = paymentRepository;
            _integrationEventPublisher = integrationEventPublisher;
            _correlationIdGenerator = correlationIdGenerator;
        }

        public IEnumerable<Payment> GetAll()
        {
            return _paymentRepository.GetAll();
        }

        public Payment? GetById(int id)
        {
            return _paymentRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public Payment? GetByPurchaseId(int purchaseId)
        {
            return _paymentRepository.GetAll().FirstOrDefault(x => x.PurchaseId == purchaseId);
        }

        public async Task<Payment> ProcessPaymentAsync(PaymentInput input)
        {
            ValidarInput(input);

            var payment = new Payment
            {
                PurchaseId = input.PurchaseId,
                UserId = input.UserId,
                Amount = input.Amount,
                Status = "Processing"
            };

            var createdPayment = _paymentRepository.Add(payment);

            var approved = SimularResultadoPagamento();

            createdPayment.Status = approved ? "Approved" : "Rejected";
            _paymentRepository.Update(createdPayment);

            var correlationId = _correlationIdGenerator.Get();

            if (approved)
            {
                await _integrationEventPublisher.PublishAsync(new PaymentApprovedEvent
                {
                    PaymentId = createdPayment.Id,
                    PurchaseId = createdPayment.PurchaseId,
                    UserId = createdPayment.UserId,
                    Amount = createdPayment.Amount,
                    CorrelationId = correlationId
                });

                Console.WriteLine(
                    $"EMAIL SIMULADO: pagamento aprovado para o usuário {createdPayment.UserId} na compra {createdPayment.PurchaseId}.");
            }
            else
            {
                await _integrationEventPublisher.PublishAsync(new PaymentRejectedEvent
                {
                    PaymentId = createdPayment.Id,
                    PurchaseId = createdPayment.PurchaseId,
                    UserId = createdPayment.UserId,
                    Amount = createdPayment.Amount,
                    CorrelationId = correlationId
                });
            }

            return createdPayment;
        }

        private static void ValidarInput(PaymentInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.PurchaseId <= 0)
                throw new Exception("PurchaseId inválido.");

            if (input.UserId <= 0)
                throw new Exception("UserId inválido.");

            if (input.Amount <= 0)
                throw new Exception("Amount deve ser maior que zero.");
        }

        private static bool SimularResultadoPagamento()
        {
            return Random.Shared.Next(1, 101) > 20;
        }
    }
}
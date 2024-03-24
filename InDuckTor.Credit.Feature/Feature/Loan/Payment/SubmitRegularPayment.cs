using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Feature.Feature.Interceptors;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitRegularPayment : ICommand<PaymentSubmission, Unit>;

[Intercept(typeof(SaveChangesInterceptor<PaymentSubmission, Unit>))]
public class SubmitRegularPayment(LoanDbContext context, IPaymentService paymentService) : ISubmitRegularPayment
{
    public async Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        var newPayment = new NewPayment(input.LoanId, input.ClientId, input.Payment);
        var payment = await paymentService.CreatePayment(newPayment, ct);
        context.Payments.Add(payment);

        await paymentService.DistributePayment(payment);

        return default;
    }
}
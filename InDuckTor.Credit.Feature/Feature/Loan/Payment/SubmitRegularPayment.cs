using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitRegularPayment : ICommand<PaymentSubmission, Unit>;

[Intercept(typeof(SaveChangesInterceptor<PaymentSubmission, Unit>))]
public class SubmitRegularPayment(LoanDbContext context, IPaymentService paymentService) : ISubmitRegularPayment
{
    public async Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        var isExists = await context.Loans.AnyAsync(
            loan => loan.Id == input.LoanId && loan.ClientId == input.ClientId, ct
        );

        if (!isExists) throw new Errors.Loan.NotFound("Loan with specified client id and loan id is not found");

        var payment = paymentService.CreatePayment(new NewPayment(input.LoanId, input.ClientId, input.Payment));
        context.Payments.Add(payment);

        await paymentService.DistributePayment(payment);

        return default;
    }
}
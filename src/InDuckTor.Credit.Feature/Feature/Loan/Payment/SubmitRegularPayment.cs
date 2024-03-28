using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Feature.Feature.Interceptors;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Errors = InDuckTor.Credit.Domain.Exceptions.Errors;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitRegularPayment : ICommand<PaymentSubmission, Unit>;

[Intercept(typeof(SaveChangesInterceptor<PaymentSubmission, Unit>))]
public class SubmitRegularPayment : ISubmitRegularPayment
{
    private readonly LoanDbContext _context;
    private readonly IPaymentService _paymentService;

    public SubmitRegularPayment(LoanDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        var newPayment = new NewPayment(input.LoanId, input.ClientId, input.Payment);
        var payment = await _paymentService.CreatePayment(newPayment, ct);
        _context.Payments.Add(payment);

        var loan = await _context.Loans.FindAsync([input.LoanId], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(input.LoanId);
        await _paymentService.DistributePayment(loan, payment);

        return default;
    }
}
using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Feature.Feature.Interceptors;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Errors = InDuckTor.Credit.Domain.Exceptions.Errors;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitRegularPayment : ICommand<PaymentSubmissionRequest, PaymentSubmissionResponse>;

[Intercept(typeof(SaveChangesInterceptor<PaymentSubmissionRequest, PaymentSubmissionResponse>))]
public class SubmitRegularPayment : ISubmitRegularPayment
{
    private readonly LoanDbContext _context;
    private readonly IPaymentService _paymentService;

    public SubmitRegularPayment(LoanDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<PaymentSubmissionResponse> Execute(PaymentSubmissionRequest input, CancellationToken ct)
    {
        var loan = await _context.Loans.FindAsync([input.LoanId], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(input.LoanId);

        if (loan.Id != input.LoanId || loan.ClientId != input.ClientId)
            throw new Errors.Loan.NotFound("Loan with specified client id and loan id is not found");

        var newPayment = new NewPayment(input.LoanId, input.ClientId, input.Payment);
        var payment = await _paymentService.CreatePayment(loan, newPayment, ct);
        _context.Payments.Add(payment);

        await _paymentService.DistributePayment(loan, payment);

        return new PaymentSubmissionResponse(payment.LoanId, payment.ClientId, payment.PaymentAmount);
    }
}
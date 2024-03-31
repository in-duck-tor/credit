using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
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
    private readonly IAccountsRepository _accountsRepository;

    public SubmitRegularPayment(LoanDbContext context,
        IPaymentService paymentService,
        IAccountsRepository accountsRepository)
    {
        _context = context;
        _paymentService = paymentService;
        _accountsRepository = accountsRepository;
    }

    public async Task<PaymentSubmissionResponse> Execute(PaymentSubmissionRequest input, CancellationToken ct)
    {
        var loan = await _context.Loans.FindAsync([input.LoanId], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(input.LoanId);

        if (loan.Id != input.LoanId || loan.ClientId != input.ClientId)
            throw new Errors.Loan.NotFound(input.ClientId, input.LoanId);

        var newPayment = new NewPayment(input.LoanId, input.ClientId, input.Payment);
        var payment = await _paymentService.CreatePayment(loan, newPayment, ct);
        _context.Payments.Add(payment);

        ArgumentNullException.ThrowIfNull(loan.LoanAccountNumber);
        var transactionInfo = await InitiateTransaction(
            input.ClientId,
            payment.PaymentAmount,
            loan.LoanAccountNumber,
            loan.ClientAccountNumber);

        await _paymentService.DistributePayment(loan, payment);

        await _accountsRepository.CommitTransaction(transactionInfo.TransactionId);

        return new PaymentSubmissionResponse(payment.LoanId, payment.ClientId, payment.PaymentAmount);
    }

    private async Task<TransactionInfo> InitiateTransaction(
        long clientId,
        decimal paymentAmount,
        string loanAccountNumber,
        string clientAccountNumber)
    {
        if (!await _accountsRepository.IsAccountOwner(clientId, clientAccountNumber))
            throw new Errors.Transaction.CannotInitiateTransaction.ClientIsNotAccountOwner();

        var newTransaction = NewTransaction.ForInDuckTor(
            paymentAmount,
            loanAccountNumber,
            clientAccountNumber,
            executeImmediate: false
        );

        var transactionInfo = await _accountsRepository.InitiateTransaction(newTransaction);
        if (transactionInfo.Status == TransactionStatus.Canceled)
            throw new Errors.Transaction.CannotInitiateTransaction.Unknown();

        return transactionInfo;
    }
}
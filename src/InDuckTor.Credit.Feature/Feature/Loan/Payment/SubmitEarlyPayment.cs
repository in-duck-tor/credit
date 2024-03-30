using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitEarlyPayment : ICommand<PaymentSubmissionRequest, Unit>;

public class SubmitEarlyPayment : ISubmitEarlyPayment
{
    public Task<Unit> Execute(PaymentSubmissionRequest input, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
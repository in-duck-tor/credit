using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitEarlyPayment : ICommand<PaymentSubmissionRequest, PaymentSubmissionResponse>;

public class SubmitEarlyPayment : ISubmitEarlyPayment
{
    public Task<PaymentSubmissionResponse> Execute(PaymentSubmissionRequest input, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public interface ISubmitEarlyPayment : ICommand<PaymentSubmission, Unit>;

public class SubmitEarlyPayment : ISubmitEarlyPayment
{
    public Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
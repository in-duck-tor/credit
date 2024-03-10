using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public interface ISubmitEarlyPayment : ICommand<PaymentSubmission, Unit>;

public class SubmitEarlyPayment : ISubmitEarlyPayment
{
    public Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        // todo
        throw new NotImplementedException();
    }
}
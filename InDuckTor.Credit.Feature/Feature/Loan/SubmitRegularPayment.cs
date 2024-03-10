using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public interface ISubmitRegularPayment : ICommand<PaymentSubmission, Unit>;

public class SubmitRegularPayment : ISubmitRegularPayment
{
    public Task<Unit> Execute(PaymentSubmission input, CancellationToken ct)
    {
        // todo
        throw new NotImplementedException();
    }
}
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public record LoanInfoShortResponse(
    decimal BorrowedAmount,
    decimal InterestRate,
    int PlannedPaymentsNumber,
    decimal LoanBody,
    decimal LoanDebt,
    decimal Penalty);

public interface IGetAllClientLoans : IQuery<long, LoanInfoShortResponse>;

public class GetAllClientLoans : IGetAllClientLoans
{
    /// <param name="input">Id клиента, для которого выполняется запрос</param>
    /// <param name="ct"></param>
    public Task<LoanInfoShortResponse> Execute(long input, CancellationToken ct)
    {
        // todo
        throw new NotImplementedException();
    }
}
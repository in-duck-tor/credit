using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public record LoanInfoResponse(
    decimal BorrowedAmount,
    decimal InterestRate,
    DateTime ApprovalDate,
    DateTime? BorrowingDate,
    int PlannedPaymentsNumber,
    PaymentType PaymentType,
    decimal LoanBody,
    decimal LoanDebt,
    decimal Penalty);

public interface IGetLoanInfo : IQuery<long, LoanInfoResponse>;

public class GetLoanInfo : IGetLoanInfo
{
    public Task<LoanInfoResponse> Execute(long input, CancellationToken ct)
    {
        // todo
        throw new NotImplementedException();
    }
}
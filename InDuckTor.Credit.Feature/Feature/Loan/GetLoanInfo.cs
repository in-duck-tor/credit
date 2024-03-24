using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;
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
    decimal Penalty)
{
    public static LoanInfoResponse FromLoan(Domain.LoanManagement.Loan loan) => new(
        loan.BorrowedAmount,
        loan.InterestRate,
        loan.ApprovalDate,
        loan.BorrowingDate,
        loan.PlannedPaymentsNumber,
        loan.PaymentType,
        loan.LoanBilling.LoanBody,
        loan.LoanBilling.LoanDebt,
        loan.LoanBilling.Penalty
    );
}

/// <summary>
/// Возвращает информацию о Кредите
/// </summary>
public interface IGetLoanInfo : ICommand<long, LoanInfoResponse>;

public class GetLoanInfo(LoanDbContext context) : IGetLoanInfo
{
    public async Task<LoanInfoResponse> Execute(long loanId, CancellationToken ct)
    {
        var loan = await context.Loans.FindAsync([loanId], cancellationToken: ct)
                   ?? throw new Errors.LoanProgram.NotFound(loanId);
        return LoanInfoResponse.FromLoan(loan);
    }
}
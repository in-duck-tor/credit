using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

/// <param name="Id">Id Кредита</param>
/// <param name="BorrowedAmount">Сумма займа</param>
/// <param name="InterestRate">Процентная ставка</param>
/// <param name="State">Статус Кредита</param>
/// <param name="LoanBody">Остаток по телу кредита</param>
/// <param name="LoanDebt">Сумма Задолженности по Кредиту</param>
/// <param name="Penalty">Штраф по Задолженности</param>
public record LoanInfoShortResponse(
    long Id,
    decimal BorrowedAmount,
    decimal InterestRate,
    LoanState State,
    decimal LoanBody,
    decimal LoanDebt,
    decimal Penalty)
{
    public static LoanInfoShortResponse FromLoan(Domain.LoanManagement.Loan loan) => new(
        loan.Id,
        loan.BorrowedAmount,
        loan.InterestRate,
        loan.State,
        loan.CurrentBody,
        loan.Debt,
        loan.Penalty
    );
}

public interface IGetAllClientLoans : IQuery<long, List<LoanInfoShortResponse>>;

public class GetAllClientLoans(LoanDbContext context) : IGetAllClientLoans
{
    public async Task<List<LoanInfoShortResponse>> Execute(long clientId, CancellationToken ct)
    {
        return await context.Loans
            .Where(loan => loan.ClientId == clientId)
            .Select(loan => LoanInfoShortResponse.FromLoan(loan))
            .ToListAsync(cancellationToken: ct);
    }
}
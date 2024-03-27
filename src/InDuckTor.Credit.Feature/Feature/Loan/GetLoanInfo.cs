using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

/// <param name="Id">Id Кредита</param>
/// <param name="BorrowedAmount">Сумма займа</param>
/// <param name="InterestRate">Процентная ставка</param>
/// <param name="ClientAccountNumber">Счёт клиента</param>
/// <param name="ApprovalDate">Дата одобрения кредита</param>
/// <param name="BorrowingDate">Дата начисления кредитных средств</param>
/// <param name="PlannedPaymentsNumber">Планируемое число платежей</param>
/// <param name="PaymentType">Тип Платежа</param>
/// <param name="LoanBody">Остаток по телу кредита</param>
/// <param name="LoanDebt">Сумма Задолженности по Кредиту</param>
/// <param name="Penalty">Штраф по Задолженности</param>
public record LoanInfoResponse(
    long Id,
    decimal BorrowedAmount,
    string InterestRate,
    string ClientAccountNumber,
    DateTime ApprovalDate,
    DateTime? BorrowingDate,
    int PlannedPaymentsNumber,
    PaymentType PaymentType,
    decimal LoanBody,
    decimal LoanDebt,
    decimal Penalty)
{
    public static LoanInfoResponse FromLoan(Domain.LoanManagement.Loan loan) => new(
        loan.Id,
        loan.BorrowedAmount,
        loan.InterestRate * 100 + "%",
        loan.ClientAccountNumber,
        loan.ApprovalDate,
        loan.BorrowingDate,
        loan.PlannedPaymentsNumber,
        loan.PaymentType,
        loan.Body,
        loan.Debt,
        loan.Penalty
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
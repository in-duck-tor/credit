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
/// <param name="State">Статус Кредита</param>
/// <param name="PaymentType">Тип Платежа</param>
/// <param name="PeriodInterest">Проценты по текущему периоду</param>
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
    LoanState State,
    PaymentType PaymentType,
    decimal PeriodInterest,
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
        loan.State,
        loan.PaymentType,
        loan.AccruedInterest,
        loan.CurrentBody,
        loan.Debt,
        loan.Penalty
    );
}

/// <summary>
/// Возвращает информацию о Кредите
/// </summary>
public interface IGetLoanInfo : IQuery<long, LoanInfoResponse>;

public class GetLoanInfo(LoanDbContext context) : IGetLoanInfo
{
    // todo: Добавить проверку id клиента из токена
    public async Task<LoanInfoResponse> Execute(long loanId, CancellationToken ct)
    {
        var loan = await context.Loans.FindAsync([loanId], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(loanId);
        return LoanInfoResponse.FromLoan(loan);
    }
}
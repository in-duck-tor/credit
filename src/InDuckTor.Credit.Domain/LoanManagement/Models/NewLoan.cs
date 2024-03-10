using InDuckTor.Credit.Domain.Financing.Application;

namespace InDuckTor.Credit.Domain.LoanManagement.Models;

public record NewLoan(
    long ClientId,
    string ClientAccountNumber,
    decimal BorrowedAmount,
    decimal InterestRate,
    DateTime ApprovalDate,
    TimeSpan LoanTerm,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType,
    TimeSpan? PeriodInterval)
{
    public static NewLoan FromApplication(LoanApplication la)
    {
        ArgumentNullException.ThrowIfNull(la.ApprovalDate);
        return new NewLoan(
            la.ClientId,
            la.ClientAccountNumber,
            la.BorrowedAmount,
            la.LoanProgram.InterestRate,
            la.ApprovalDate.Value,
            la.LoanTerm,
            la.LoanProgram.PaymentType,
            la.LoanProgram.PaymentScheduleType,
            la.LoanProgram.PeriodInterval
        );
    }
}
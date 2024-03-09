namespace InDuckTor.Credit.Domain.LoanManagement.Models;

public record NewLoan(
    decimal BorrowedAmount,
    decimal InterestRate,
    DateTime ApprovalDate,
    TimeSpan LoanTerm,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
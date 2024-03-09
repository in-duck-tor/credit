namespace InDuckTor.Credit.Domain.LoanManagement.Model;

public record NewLoan(
    decimal BorrowedAmount,
    decimal InterestRate,
    DateTime ApprovalDate,
    TimeSpan LoanTerm,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
namespace InDuckTor.Credit.Domain.Loan.Service.Model;

public record NewLoan(
    decimal BorrowedAmount,
    decimal InterestRate,
    DateTime ApprovalDate,
    DateTime LoanTerm,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
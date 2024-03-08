using InDuckTor.Credit.Domain.Loan;

namespace InDuckTor.Credit.WebApi.Contracts.Responses;

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
namespace InDuckTor.Credit.WebApi.Contracts.Responses;

public record LoanInfoShortResponse(
    decimal BorrowedAmount,
    decimal InterestRate,
    int PlannedPaymentsNumber,
    decimal LoanBody,
    decimal LoanDebt,
    decimal Penalty);
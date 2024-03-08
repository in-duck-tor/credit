namespace InDuckTor.Credit.WebApi.Contracts.Bodies;

public record CreateLoanApplicationBody(
    long ClientId,
    long LoanProgramId,
    decimal BorrowedAmount,
    DateTime LoanTerm);
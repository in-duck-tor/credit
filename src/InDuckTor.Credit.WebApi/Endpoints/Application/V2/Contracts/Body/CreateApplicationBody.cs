namespace InDuckTor.Credit.WebApi.Endpoints.Application.V2.Contracts.Body;

public record CreateApplicationBody(
    long LoanProgramId,
    decimal BorrowedAmount,
    long LoanTerm,
    string ClientAccountNumber);
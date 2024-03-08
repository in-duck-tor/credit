namespace InDuckTor.Credit.WebApi.Endpoints.Application.Models.Request;

public record CreateLoanApplicationRequest(
    long ClientId,
    long LoanProgramId,
    decimal BorrowedAmount,
    DateTime LoanTerm);
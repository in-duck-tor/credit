namespace InDuckTor.Credit.WebApi.Endpoints.LoanApplication.Models.Request;

public record CreateLoanApplicationRequest(
    long ClientId,
    long LoanProgramId,
    decimal BorrowedAmount,
    DateTime LoanTerm);
using InDuckTor.Credit.Domain.Loan.Financing.Application;

namespace InDuckTor.Credit.WebApi.Endpoints.LoanApplication.Models;

public record CreateLoanApplicationDto(
    long LoanProgramId,
    decimal BorrowedAmount,
    DateTime LoanTerm);
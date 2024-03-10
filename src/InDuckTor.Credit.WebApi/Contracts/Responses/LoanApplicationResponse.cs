using InDuckTor.Credit.Domain.Financing.Application;

namespace InDuckTor.Credit.WebApi.Contracts.Responses;

public class LoanApplicationResponse(
    long ClientId,
    LoanProgramShortResponse LoanProgram,
    decimal BorrowedAmount,
    DateTime LoanTerm,
    ApplicationState ApplicationState);
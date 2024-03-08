using InDuckTor.Credit.WebApi.Contracts.Dtos;

namespace InDuckTor.Credit.WebApi.Contracts.Responses;

public class LoanApplicationResponse(
    long ClientId,
    LoanProgramShortResponse LoanProgram,
    decimal BorrowedAmount,
    DateTime LoanTerm,
    ApplicationStateDto ApplicationState);
namespace InDuckTor.Credit.WebApi.Contracts.Bodies;

public record ApplicationInfoBodyV2(
    long LoanProgramId,
    decimal BorrowedAmount,
    long LoanTerm,
    string ClientAccountNumber);
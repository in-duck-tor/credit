using InDuckTor.Credit.Domain.Loan;

namespace InDuckTor.Credit.WebApi.Contracts.Responses;

public record LoanProgramShortResponse(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
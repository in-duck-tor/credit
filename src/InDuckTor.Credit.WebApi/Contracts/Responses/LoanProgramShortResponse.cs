using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.WebApi.Contracts.Responses;

public record LoanProgramShortResponse(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
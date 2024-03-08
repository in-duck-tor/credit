using InDuckTor.Credit.Domain.Loan;

namespace InDuckTor.Credit.WebApi.Endpoints.LoanProgram.Model.Response;

public record LoanProgramShortResponse(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
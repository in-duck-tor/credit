using InDuckTor.Credit.Domain.Loan;

namespace InDuckTor.Credit.WebApi.Endpoints.Program.Model.Response;

public record LoanProgramShortResponse(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
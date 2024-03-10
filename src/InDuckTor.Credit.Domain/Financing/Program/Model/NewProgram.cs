using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Financing.Program.Model;

public record NewProgram(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType,
    TimeSpan PeriodInterval
);
namespace InDuckTor.Credit.Domain.Loan.Financing.Program.Model;

public record NewProgram(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);
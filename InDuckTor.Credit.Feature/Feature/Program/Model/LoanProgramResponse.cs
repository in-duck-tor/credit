using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Feature.Feature.Program.Model;

/// <param name="Id">Id сущности</param>
/// <param name="InterestRate">Процентная ставка в процентах</param>
/// <param name="PaymentType">Тип платежа: Аннуитетный или Дифференцированный</param>
/// <param name="PaymentScheduleType">Тип кредитного графика (сейчас всегда выбирать interval)</param>
/// <param name="PeriodInterval">Длительность Расчётного Периода (для интервального графика)</param>
public record LoanProgramResponse(
    long Id,
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType,
    TimeSpan? PeriodInterval)
{
    public static LoanProgramResponse FromLoanProgram(LoanProgram loanProgram) => new(
        loanProgram.Id,
        loanProgram.InterestRate * 100,
        loanProgram.PaymentType,
        loanProgram.PaymentScheduleType,
        loanProgram.PeriodInterval);
}
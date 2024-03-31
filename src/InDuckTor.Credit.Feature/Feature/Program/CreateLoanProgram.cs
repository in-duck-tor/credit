using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.Financing.Program.Model;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Feature.Feature.Interceptors;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Program;

/// <param name="InterestRate">Процентная ставка в процентах</param>
/// <param name="PaymentType">Тип платежа: Аннуитетный или Дифференцированный</param>
/// <param name="PaymentScheduleType">Тип кредитного графика (сейчас всегда выбирать interval)</param>
/// <param name="PeriodInterval">Длительность Расчётного Периода (для интервального графика)</param>
public record LoanProgramInfoRequest(
    [property: Required] decimal InterestRate,
    [property: Required] PaymentType PaymentType,
    [property: Required] PaymentScheduleType PaymentScheduleType,
    [property: Required] long PeriodInterval
);

public interface ICreateLoanProgram : ICommand<LoanProgramInfoRequest, LoanProgramResponse>;

[Intercept(typeof(SaveChangesInterceptor<LoanProgramInfoRequest, LoanProgramResponse>))]
public class CreateLoanProgram(LoanDbContext context, ILoanProgramService loanProgramService) : ICreateLoanProgram
{
    public async Task<LoanProgramResponse> Execute(LoanProgramInfoRequest input, CancellationToken ct)
    {
        var newProgram = new NewProgram(
            input.InterestRate / 100,
            input.PaymentType,
            input.PaymentScheduleType,
            TimeSpan.FromSeconds(input.PeriodInterval));
        var loanProgram = loanProgramService.CreateProgram(newProgram);

        context.Add(loanProgram);
        await context.SaveChangesAsync(ct);

        return LoanProgramResponse.FromLoanProgram(loanProgram);
    }
}
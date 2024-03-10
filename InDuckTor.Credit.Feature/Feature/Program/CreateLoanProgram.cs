using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.Financing.Program.Model;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Program;

/// <param name="InterestRate">Процентная ставка в процентах</param>
/// <param name="PaymentType">Тип платежа: Аннуитетный или Дифференцированный</param>
/// <param name="PaymentScheduleType">Тип кредитного графика (сейчас всегда выбирать interval)</param>
/// <param name="PeriodInterval">Длительность Расчётного Периода (для интервального графика)</param>
public record LoanProgramInfo(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType,
    long PeriodInterval
);

public interface ICreateLoanProgram : ICommand<LoanProgramInfo, LoanProgramResponse>;

[Intercept(typeof(SaveChangesInterceptor<LoanProgramInfo, LoanProgramResponse>))]
public class CreateLoanProgram(LoanDbContext context, ILoanProgramService loanProgramService) : ICreateLoanProgram
{
    public async Task<LoanProgramResponse> Execute(LoanProgramInfo input, CancellationToken ct)
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
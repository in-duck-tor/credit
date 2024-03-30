using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

/// <param name="PeriodStartDate">Дата начала периода</param>
/// <param name="PeriodEndDate">Дата окончания периода</param>
/// <param name="OneTimePayment">Сумма единовременного платежа по периоду</param>
/// <param name="RemainingPayment">Оставшийся платёж по периоду</param>
public record PeriodInfo(
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    decimal OneTimePayment,
    decimal RemainingPayment);

public interface IGetOverduePeriods : IQuery<long, List<PeriodInfo>>;

public class GetOverduePeriods(LoanDbContext context) : IGetOverduePeriods
{
    // todo: Добавить проверку id клиента из токена
    public async Task<List<PeriodInfo>> Execute(long loanId, CancellationToken ct)
    {
        return await context.PeriodsBillings
            .Where(pb => pb.Loan.Id == loanId && pb.IsDebt)
            .Select(pb =>
                new PeriodInfo(pb.PeriodStartDate, pb.PeriodEndDate, pb.OneTimePayment, pb.TotalRemainingPayment))
            .ToListAsync(ct);
    }
}
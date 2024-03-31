using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

/// <param name="PeriodStartDate">Дата начала периода</param>
/// <param name="PeriodEndDate">Дата окончания периода</param>
/// <param name="OneTimePayment">Сумма единовременного платежа по периоду</param>
/// <param name="RemainingPayment">Оставшийся платёж по периоду</param>
public record PeriodInfoResponse(
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    MoneyView OneTimePayment,
    MoneyView RemainingPayment);

public interface IGetOverduePeriodsV1 : IQuery<long, List<PeriodInfoResponse>>;

public class GetOverduePeriodsV1(LoanDbContext context) : IGetOverduePeriodsV1
{
    public async Task<List<PeriodInfoResponse>> Execute(long loanId, CancellationToken ct)
    {
        return await context.PeriodsBillings
            .Where(pb => pb.Loan.Id == loanId && pb.IsDebt)
            .Select(pb =>
                new PeriodInfoResponse(pb.PeriodStartDate, pb.PeriodEndDate, pb.OneTimePayment, pb.TotalRemainingPayment))
            .ToListAsync(ct);
    }
}
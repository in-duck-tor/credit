using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public record GetOverduePeriodsRequest(long LoanId, long ClientId);

public interface IGetOverduePeriodsV2 : IQuery<GetOverduePeriodsRequest, List<PeriodInfoResponse>>;

public class GetOverduePeriodsV2(LoanDbContext context) : IGetOverduePeriodsV2
{
    public async Task<List<PeriodInfoResponse>> Execute(GetOverduePeriodsRequest request, CancellationToken ct)
    {
        return await context.PeriodsBillings
            .Where(pb => pb.Loan.Id == request.LoanId && pb.Loan.ClientId == request.ClientId && pb.IsDebt)
            .Select(pb =>
                new PeriodInfoResponse(pb.PeriodStartDate, pb.PeriodEndDate, pb.OneTimePayment,
                    pb.TotalRemainingPayment))
            .ToListAsync(ct);
    }
}
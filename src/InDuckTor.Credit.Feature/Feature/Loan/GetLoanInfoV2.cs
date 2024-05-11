using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.LoanManagement.State;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public record GetLoanInfoRequestV2(long ClientId, long LoanId);

/// <summary>
/// Возвращает информацию о Кредите
/// </summary>
public interface IGetLoanInfoV2 : IQuery<GetLoanInfoRequestV2, LoanInfoResponse>;

public class GetLoanInfoV2(LoanDbContext context) : IGetLoanInfoV2
{
    public async Task<LoanInfoResponse> Execute(GetLoanInfoRequestV2 request, CancellationToken ct)
    {
        var loan = await context.Loans
                       .Where(l => l.Id == request.LoanId && l.ClientId == request.ClientId)
                       .FirstOrDefaultAsync(ct)
                   ?? throw new Errors.Loan.NotFound(request.ClientId, request.LoanId);
        return LoanInfoResponse.FromLoan(loan);
    }
}
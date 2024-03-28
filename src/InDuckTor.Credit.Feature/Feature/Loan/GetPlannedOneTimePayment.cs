using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

/// <param name="CurrentPeriodPayment">Сумма единовременного платежа за период</param>
/// <param name="TotalDebt">Задолженность по Кредиту</param>
/// <param name="MaxRegularPayment">Наибольшая сумма, которую можно внести в рамках регулярного платежа. Превышение этой суммы приведёт к ошибке</param>
public record PaymentInfoResponse(decimal CurrentPeriodPayment, decimal TotalDebt, decimal MaxRegularPayment);

public interface IGetPaymentInfo : IQuery<long, PaymentInfoResponse>;

public class GetPaymentInfo(LoanDbContext context, IPeriodBillingRepository periodBillingRepository) : IGetPaymentInfo
{
    public async Task<PaymentInfoResponse> Execute(long loanId, CancellationToken ct)
    {
        var loan = await context.Loans.FindAsync([loanId, ct], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(loanId);

        var unpaidPeriods = await periodBillingRepository.GetAllUnpaidPeriodBillings(loan.Id);
        var totalDebt = unpaidPeriods
            .Select(periodBilling => periodBilling.TotalRemainingPayment)
            .Sum();
        var maxRegularPayment = totalDebt + loan.GetPlannedOneTimePayment();

        return new PaymentInfoResponse(loan.GetPlannedOneTimePayment(), totalDebt, maxRegularPayment);
    }
}
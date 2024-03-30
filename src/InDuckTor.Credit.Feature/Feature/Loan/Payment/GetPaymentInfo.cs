using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

/// <param name="RequiredCurrentPeriodPayment">Сумма единовременного платежа за период</param>
/// <param name="Debt">Задолженность по Кредиту</param>
/// <param name="TotalRemainingPayment">Общая сумма платежа на данный момент. Превышение этой суммы приведёт к ошибке</param>
public record PaymentInfoResponse(
    MoneyView RequiredCurrentPeriodPayment,
    MoneyView Debt,
    MoneyView TotalRemainingPayment);

public interface IGetPaymentInfo : IQuery<long, PaymentInfoResponse>;

public class GetPaymentInfo(LoanDbContext context, IPaymentRepository paymentRepository) : IGetPaymentInfo
{
    public async Task<PaymentInfoResponse> Execute(long loanId, CancellationToken ct)
    {
        var loan = await context.Loans.FindAsync([loanId, ct], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(loanId);
        var undistributedPayments = await paymentRepository.GetAllNonDistributedPayments(loan.Id);

        // todo: подумать над тем, чтобы перенести платежи в кредит и проводить этот расчёт внутри кредита
        var totalRemainingPayment = loan.GetCurrentTotalPayment();
        var totalPaymentToDistributeSum = undistributedPayments.Select(p => p.PaymentToDistribute).Sum();

        return new PaymentInfoResponse(
            loan.GetExpectedOneTimePayment(),
            loan.Debt,
            totalRemainingPayment - totalPaymentToDistributeSum);
    }
}
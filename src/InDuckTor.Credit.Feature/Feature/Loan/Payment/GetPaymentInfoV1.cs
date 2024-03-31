using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

/// <param name="PeriodPayment">Сумма единовременного платежа за период</param>
/// <param name="Debt">Задолженность по Кредиту</param>
/// <param name="RemainingRegularPayment">Общая сумма платежа на данный момент. Превышение этой суммы приведёт к ошибке</param>
public record PaymentInfoResponse(
    MoneyView PeriodPayment,
    MoneyView Debt,
    MoneyView RemainingRegularPayment,
    TimeSpan TimeUntilPeriodEnd);

public interface IGetPaymentInfoV1 : IQuery<long, PaymentInfoResponse>;

public class GetPaymentInfoV1(LoanDbContext context, IPaymentRepository paymentRepository) : IGetPaymentInfoV1
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
            totalRemainingPayment - totalPaymentToDistributeSum,
            loan.TimeUntilPeriodEnd);
    }
}
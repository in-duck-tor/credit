using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment;

public record GetPaymentInfoRequestV2(long LoanId, long ClientId);

public interface IGetPaymentInfoV2 : IQuery<GetPaymentInfoRequestV2, PaymentInfoResponse>;

public class GetPaymentInfoV2(LoanDbContext context, IPaymentRepository paymentRepository) : IGetPaymentInfoV2
{
    public async Task<PaymentInfoResponse> Execute(GetPaymentInfoRequestV2 request, CancellationToken ct)
    {
        var loan = await context.Loans.FindAsync([request.LoanId, ct], cancellationToken: ct)
                   ?? throw new Errors.Loan.NotFound(request.ClientId, request.LoanId);
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
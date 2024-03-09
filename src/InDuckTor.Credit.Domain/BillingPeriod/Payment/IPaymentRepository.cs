namespace InDuckTor.Credit.Domain.BillingPeriod.Payment;

public interface IPaymentRepository
{
    List<Payment> GetAllNonDistributedPayments(long loanId);
}
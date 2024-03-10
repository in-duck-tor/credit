namespace InDuckTor.Credit.Domain.Billing.Payment;

public interface IPaymentRepository
{
    List<Payment> GetAllNonDistributedPayments(long loanId);
}
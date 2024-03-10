namespace InDuckTor.Credit.Domain.Billing.Payment;

public interface IPaymentRepository
{
    Task<List<Payment>> GetAllNonDistributedPayments(long loanId);
}
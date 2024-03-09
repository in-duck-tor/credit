namespace InDuckTor.Credit.Domain.Payment;

public interface IPaymentRepository
{
    List<Payment> GetAllNonDistributedPayments(long loanId);
}
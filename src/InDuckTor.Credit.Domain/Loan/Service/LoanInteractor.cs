using InDuckTor.Credit.Domain.Loan.PaymentSystem;

namespace InDuckTor.Credit.Domain.Loan.Service;

public class LoanInteractor
{
    public required Loan Loan { get; set; }
    public required IPaymentSystem PaymentSystem { get; set; }
}
using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement.Interactor;

public class LoanInteractor
{
    public required Loan Loan { get; set; }
    public required IPaymentCalculator PaymentCalculator { get; set; }
}
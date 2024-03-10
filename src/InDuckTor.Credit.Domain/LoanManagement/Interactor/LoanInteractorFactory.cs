using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement.Interactor;

public interface ILoanInteractorFactory
{
    LoanInteractor FromLoan(Loan loan);
}

public class LoanInteractorFactory : ILoanInteractorFactory
{
    public LoanInteractor FromLoan(Loan loan)
    {
        return new LoanInteractor(loan, GetPaymentSystem(loan.PaymentType));
    }

    private static IPaymentCalculator GetPaymentSystem(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Annuity => new AnnuityPaymentCalculator(),
            PaymentType.Differentiated => new DifferentiatedPaymentCalculator(),
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }
}
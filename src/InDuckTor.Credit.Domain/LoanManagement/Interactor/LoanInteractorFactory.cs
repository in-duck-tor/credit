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
        return new LoanInteractor(loan, GetPaymentSystem(loan));
    }

    private static IPaymentCalculator GetPaymentSystem(Loan loan)
    {
        return loan.PaymentType switch
        {
            PaymentType.Annuity => new AnnuityPaymentCalculator(loan),
            PaymentType.Differentiated => new DifferentiatedPaymentCalculator(loan),
            _ => throw new ArgumentOutOfRangeException(nameof(loan.PaymentType))
        };
    }
}
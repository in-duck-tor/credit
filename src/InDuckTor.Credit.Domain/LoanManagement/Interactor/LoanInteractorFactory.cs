using InDuckTor.Credit.Domain.LoanManagement.Models;
using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement.Interactor;

public interface ILoanInteractorFactory
{
    LoanInteractor FromNewLoan(NewLoan newLoan);
    LoanInteractor FromLoan(Loan loan);
}

public class LoanInteractorFactory : ILoanInteractorFactory
{
    public LoanInteractor FromNewLoan(NewLoan newLoan)
    {
        var loan = new Loan
        {
            BorrowedAmount = newLoan.BorrowedAmount,
            InterestRate = newLoan.InterestRate,
            ApprovalDate = newLoan.ApprovalDate,
            PaymentType = newLoan.PaymentType,
            PaymentScheduleType = newLoan.PaymentScheduleType,
            State = LoanState.Approved,
            LoanBody = newLoan.BorrowedAmount,
            // todo назначать количество платежей
            //  количество платежей зависит от типа платежа по кредиту и графика платежей
            PlannedPaymentsNumber = 0,
        };

        return FromLoan(loan);
    }

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
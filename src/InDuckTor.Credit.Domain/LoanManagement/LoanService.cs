using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.LoanManagement.Interactor;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

public class LoanService(
    ILoanInteractorFactory loanInteractorFactory,
    ILoanRepository loanRepository,
    PeriodService periodService)
{
    public Loan CreateLoan(NewLoan newLoan)
    {
        // newLoan.LoanTerm;

        var loanInteractor = loanInteractorFactory.FromNewLoan(newLoan);

        // 1. Присвоить поля из LoanInfo в соответствующие им поля из Loan
        // 2. Определить планируемое количетсво платежей
        // 3. Привязать кредит к расчётному счёту
        // 4. Создать ссудный счёт


        return default!;
    }

    public void CloseBillingPeriod(long loanId)
    {
        var loan = loanRepository.GetById(loanId);
        var periodBilling = periodService.CloseBillingPeriod(loan.LoanBilling, DateTime.Now);
    }

    public void Tick(Loan loan)
    {
        var interactor = loanInteractorFactory.FromLoan(loan);

        interactor.AccrueInterestOnCurrentPeriod();
        interactor.ChargePenalty();
    }
}
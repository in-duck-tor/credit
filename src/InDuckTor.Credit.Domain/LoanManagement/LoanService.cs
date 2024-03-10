using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using InDuckTor.Credit.Domain.LoanManagement.Interactor;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanService
{
    public Task<Loan> CreateLoan(NewLoan newLoan);
    public Task Tick(Loan loan);
}

public class LoanService(
    ILoanInteractorFactory loanInteractorFactory,
    PeriodService periodService,
    IAccountsRepository accountsRepository) : ILoanService
{
    public async Task<Loan> CreateLoan(NewLoan newLoan)
    {
        var loan = Loan.CreateNewLoan(newLoan);

        // Создание ссудного счёта для Кредита
        var newAccount = CreateNewLoanAccount(loan);

        var accountNumber = await accountsRepository.CreateAccount(newAccount);
        // todo: Также здесь должен происходить перевод денег на счёт заёмщика, но я хз, как это сделать
        loan.AttachLoanAccount(accountNumber);

        return loan;
    }

    public async Task Tick(Loan loan)
    {
        var interactor = loanInteractorFactory.FromLoan(loan);

        interactor.AccrueInterestOnCurrentPeriod();
        interactor.ChargePenalty();

        if (interactor.Loan.IsCurrentPeriodEnded())
        {
            await CloseBillingPeriod(interactor);
        }
    }

    private static NewAccount CreateNewLoanAccount(Loan loan)
    {
        var extendedLoanTerm = loan.PeriodDuration() * (loan.PlannedPaymentsNumber + 2);
        var plannedExpiration = DateTime.UtcNow.Add(extendedLoanTerm);
        return new NewAccount(loan.ClientId, AccountType.Loan, "RUB", plannedExpiration);
    }

    private async Task CloseBillingPeriod(LoanInteractor interactor)
    {
        var periodBilling = await periodService.CloseBillingPeriod(interactor.Loan.LoanBilling, DateTime.UtcNow);
        if (interactor.IsRepaid) interactor.CloseLoan();
    }
}
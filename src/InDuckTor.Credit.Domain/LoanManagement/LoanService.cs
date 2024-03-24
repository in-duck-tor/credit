using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using InDuckTor.Credit.Domain.LoanManagement.Interactor;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanService
{
    public Task<Loan> CreateLoan(NewLoan newLoan);

    public Task Tick(Loan loan);
    // public Task PayRegularly(long loanId, );
}

public class LoanService(
    ILoanInteractorFactory loanInteractorFactory,
    PeriodService periodService,
    IAccountsRepository accountsRepository) : ILoanService
{
    public async Task<Loan> CreateLoan(NewLoan newLoan)
    {
        // Резервация запрошенных клиентом денег для дальнейшего перевода на расчётный счёт
        var newTransaction = new NewTransaction(
            newLoan.BorrowedAmount,
            new TransactionAccountInfo(newLoan.ClientAccountNumber, BankCodes.InDuckTorCode),
            // todo: заменить на главную кассу кредитной организации
            null
        );
        var transactionInfo = await accountsRepository.InitiateTransaction(newTransaction);

        if (transactionInfo.Status == TransactionStatus.Canceled) throw new Errors.Loan.CannotProvideLoan();

        var loan = Loan.CreateNewLoan(newLoan);

        // Создание ссудного счёта для Кредита
        var newAccount = CreateNewLoanAccount(loan);
        var accountNumber = await accountsRepository.CreateLoanAccount(newAccount);
        loan.AttachLoanAccount(accountNumber);

        // Перевод долговой суммы на расчётный счёт
        await accountsRepository.CommitTransaction(transactionInfo.Id);

        loan.State = LoanState.Active;
        loan.LoanBilling.StartNewPeriod();

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
        await periodService.CloseBillingPeriod(interactor.Loan.LoanBilling, DateTime.UtcNow);
        if (interactor.IsRepaid) interactor.CloseLoan();
    }
}
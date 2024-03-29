using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanService
{
    public Task<Loan> CreateLoan(NewLoan newLoan);

    public Task Tick(Loan loan);
    // public Task PayRegularly(long loanId, );
}

public class LoanService(PeriodService periodService, IAccountsRepository accountsRepository) : ILoanService
{
    public async Task<Loan> CreateLoan(NewLoan newLoan)
    {
        // Резервация запрошенных клиентом денег для дальнейшего перевода на расчётный счёт
        var newTransaction = new NewTransaction(
            newLoan.BorrowedAmount,
            new TransactionAccountInfo(newLoan.ClientAccountNumber, BankCodes.InDuckTorCode),
            // todo: заменить на главную кассу кредитной организации (мастер-счёт)
            null,
            executeImmediate: true
        );
        var transactionInfo = await accountsRepository.InitiateTransaction(newTransaction);

        if (transactionInfo.Status == TransactionStatus.Canceled) throw new Errors.Loan.CannotProvideLoan();

        var loan = new Loan(newLoan);

        // Создание ссудного счёта для Кредита
        var newAccount = CreateNewLoanAccount(loan);
        var accountNumber = await accountsRepository.CreateLoanAccount(newAccount);
        loan.AttachLoanAccount(accountNumber);

        // Перевод долговой суммы на расчётный счёт
        // await accountsRepository.CommitTransaction(transactionInfo.TransactionId);
        loan.BorrowingDate = DateTime.UtcNow;
        loan.ActivateLoan();

        return loan;
    }

    public async Task Tick(Loan loan)
    {
        loan.AccrueInterestOnCurrentPeriod();
        loan.ChargePenalty();

        if (loan.IsCurrentPeriodEnded())
        {
            await CloseBillingPeriod(loan);
        }
    }

    private static NewAccount CreateNewLoanAccount(Loan loan)
    {
        var extendedLoanTerm = loan.PeriodDuration() * (loan.PlannedPaymentsNumber + 2);
        var plannedExpiration = DateTime.UtcNow.Add(extendedLoanTerm);
        return new NewAccount(loan.ClientId, AccountType.Loan, "RUB", plannedExpiration);
    }

    private async Task CloseBillingPeriod(Loan loan)
    {
        // todo: Разобраться, почему изменения в платежах не сохраняются (возможно там используется другой DbContext и нужен Unit of Work)
        var periodBilling = await periodService.CloseBillingPeriod(loan);
        loan.AddNewPeriodAndRecalculate(periodBilling);
        if (loan.IsRepaid) loan.CloseLoan();
        loan.StartNewPeriod();
    }
}
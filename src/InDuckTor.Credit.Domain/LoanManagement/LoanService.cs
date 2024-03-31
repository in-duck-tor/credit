using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using InDuckTor.Credit.Domain.LoanManagement.Event;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanService
{
    public Task<Loan> CreateLoan(NewLoan newLoan);

    public Task Tick(Loan loan);
}

public class LoanService(PeriodService periodService, IAccountsRepository accountsRepository) : ILoanService
{
    public async Task<Loan> CreateLoan(NewLoan newLoan)
    {
        if (!await accountsRepository.IsAccountOwner(newLoan.ClientId, newLoan.ClientAccountNumber))
            throw new Errors.Transaction.CannotInitiateTransaction.ClientIsNotAccountOwner();

        // Резервация запрошенных клиентом денег для дальнейшего перевода на расчётный счёт
        var newTransaction = new NewTransaction(
            newLoan.BorrowedAmount,
            TransactionAccountInfo.ForInDuckTor(newLoan.ClientAccountNumber),
            TransactionAccountInfo.ForMasterAccount(),
            executeImmediate: false
        );
        var transactionInfo = await accountsRepository.InitiateTransaction(newTransaction);

        if (transactionInfo.Status == TransactionStatus.Canceled)
            throw new Errors.Transaction.CannotInitiateTransaction.Unknown();

        var loan = new Loan(newLoan);

        // Создание ссудного счёта для Кредита
        var newAccount = CreateNewLoanAccount(loan);
        var accountNumber = await accountsRepository.CreateAccount(newAccount);
        loan.AttachLoanAccount(accountNumber);

        // Перевод долговой суммы на расчётный счёт
        // await accountsRepository.CommitTransaction(transactionInfo.TransactionId);
        loan.BorrowingDate = DateTime.UtcNow;
        loan.ActivateLoan();

        await accountsRepository.CommitTransaction(transactionInfo.TransactionId);

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
        var extendedLoanTerm = loan.PeriodDuration * (loan.PlannedPaymentsNumber + 2);
        var plannedExpiration = DateTime.UtcNow.Add(extendedLoanTerm);
        return new NewAccount(loan.ClientId, AccountType.Loan, "RUB", plannedExpiration);
    }

    private async Task CloseBillingPeriod(Loan loan)
    {
        var periodBilling = await periodService.CloseBillingPeriod(loan);

        if (periodBilling.IsDebt)
        {
            loan.Debt.ChangeAmount(periodBilling.GetRemainingInterest() + periodBilling.GetRemainingLoanBodyPayoff());
            var periodNotPaid = new PeriodNotPaid(
                loan.ClientId,
                ExpectedPayment: periodBilling.ExpenseItems.GetTotalSum(),
                RemainingPayment: periodBilling.TotalRemainingPayment);
            loan.StoreEvent(periodNotPaid);
        }
        else
        {
            var periodPaid = new PeriodPaid(
                loan.ClientId,
                PeriodDuration: loan.PeriodDuration,
                TimeUntilPeriodEnd: loan.TimeUntilPeriodEnd);
            loan.StoreEvent(periodPaid);
        }

        if (loan.IsRepaid)
        {
            loan.CloseLoan();
            return;
        }

        if (loan.IsClientAhuel)
        {
            loan.SellToCollectors();
            return;
        }

        loan.StartNewPeriod();
    }
}
namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public interface IAccountsRepository
{
    Task<string> CreateLoanAccount(NewAccount newAccount);
    Task<TransactionInfo> InitiateTransaction(NewTransaction newTransaction);
    Task CommitTransaction(long transactionId);
    Task CancelTransaction(long transactionId);
}
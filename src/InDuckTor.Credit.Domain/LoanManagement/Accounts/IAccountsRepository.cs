namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public interface IAccountsRepository
{
    Task<string> CreateAccount(NewAccount newAccount);
    Task<TransactionInfo> InitiateTransaction(NewTransaction newTransaction);
}
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using Refit;

namespace InDuckTor.Credit.Feature.Repository;

public record AccountInfoResponse(string AccountNumber);

public interface IAccountsRepositoryRefit
{
    [Post("/api/v1/bank/account")]
    Task<AccountInfoResponse> CreateLoanAccount([Body] NewAccount newAccount, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction")]
    Task<TransactionInfo> InitiateTransaction([Body] NewTransaction newTransaction, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction/{transactionId}/commit")]
    Task CommitTransaction([AliasAs("transactionId")] long transactionId, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction/{transactionId}/cancel")]
    Task CancelTransaction([AliasAs("transactionId")] long transactionId, [Authorize] string token);
}

public record AccountsRepositoryConfig(string Token);

public class AccountsRepository(
    AccountsRepositoryConfig config,
    IAccountsRepositoryRefit accountsRepositoryRefit) : IAccountsRepository
{
    public async Task<string> CreateLoanAccount(NewAccount newAccount)
    {
        var accountInfoResponse = await accountsRepositoryRefit.CreateLoanAccount(newAccount, config.Token);
        return accountInfoResponse.AccountNumber;
    }

    public async Task<TransactionInfo> InitiateTransaction(NewTransaction newTransaction)
    {
        return await accountsRepositoryRefit.InitiateTransaction(newTransaction, config.Token);
    }

    public async Task CommitTransaction(long transactionId)
    {
        await accountsRepositoryRefit.CommitTransaction(transactionId, config.Token);
    }

    public async Task CancelTransaction(long transactionId)
    {
        await accountsRepositoryRefit.CancelTransaction(transactionId, config.Token);
    }
}
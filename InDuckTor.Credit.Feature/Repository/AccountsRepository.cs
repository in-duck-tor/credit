using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using Refit;

namespace InDuckTor.Credit.Feature.Repository;

public record AccountInfoResponse(string AccountNumber);

public interface IAccountsRepositoryRefit
{
    [Post("/api/v1/bank/account")]
    Task<AccountInfoResponse> CreateAccount([Body] NewAccount newAccount, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction")]
    Task<TransactionInfo> InitiateTransaction([Body] NewTransaction newTransaction, [Authorize] string token);
}

public record AccountsRepositoryConfig(string Token);

public class AccountsRepository(
    AccountsRepositoryConfig config,
    IAccountsRepositoryRefit accountsRepositoryRefit) : IAccountsRepository
{
    public async Task<string> CreateAccount(NewAccount newAccount)
    {
        var accountInfoResponse = await accountsRepositoryRefit.CreateAccount(newAccount, config.Token);
        return accountInfoResponse.AccountNumber;
    }

    public async Task<TransactionInfo> InitiateTransaction(NewTransaction newTransaction)
    {
        return await accountsRepositoryRefit.InitiateTransaction(newTransaction, config.Token);
    }
}
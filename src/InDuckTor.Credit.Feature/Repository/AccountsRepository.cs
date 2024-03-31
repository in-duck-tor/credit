using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using Refit;

// ReSharper disable ClassNeverInstantiated.Global

namespace InDuckTor.Credit.Feature.Repository;

public interface IAccountsRepositoryRefit
{
    record NewTransactionRequest(NewTransaction NewTransaction);

    record AccountSearchRequest(
        [AliasAs("ownerId")] long ClientId,
        int Take,
        int Skip,
        AccountState? AccountState = null,
        AccountType? AccountType = null);

    record AccountNumberResponse(string AccountNumber);

    public record GrantedUser(long Id, List<string> Actions);

    record AccountInfoResponse(
        string Number,
        string CurrencyCode,
        string BankCode,
        long OwnerId,
        long CreatedBy,
        long Amount,
        AccountState State,
        AccountType Type,
        string CustomComment,
        List<GrantedUser> GrantedUsers);

    record AccountSearchResponse(int Total, List<AccountInfoResponse> Items);

    [Post("/api/v1/bank/account")]
    Task<AccountNumberResponse> CreateAccount([Body] NewAccount newAccount, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction")]
    Task<TransactionInfo> InitiateTransaction([Body] NewTransactionRequest newTransactionRequest,
        [Authorize] string token);

    [Post("/api/v1/bank/account/transaction/{transactionId}/commit")]
    Task CommitTransaction([AliasAs("transactionId")] long transactionId, [Authorize] string token);

    [Post("/api/v1/bank/account/transaction/{transactionId}/cancel")]
    Task CancelTransaction([AliasAs("transactionId")] long transactionId, [Authorize] string token);

    Task<AccountSearchResponse> SearchAccounts([Body] AccountSearchRequest accountSearchRequest,
        [Authorize] string token);
}

public record AccountsRepositoryConfig(string Token);

public class AccountsRepository(
    AccountsRepositoryConfig config,
    IAccountsRepositoryRefit accountsRepositoryRefit) : IAccountsRepository
{
    private const int MaxTake = 1000;

    public async Task<string> CreateAccount(NewAccount newAccount)
    {
        var accountInfoResponse = await accountsRepositoryRefit.CreateAccount(newAccount, config.Token);
        return accountInfoResponse.AccountNumber;
    }

    public async Task<TransactionInfo> InitiateTransaction(NewTransaction newTransaction)
    {
        return await accountsRepositoryRefit.InitiateTransaction(
            new IAccountsRepositoryRefit.NewTransactionRequest(newTransaction),
            config.Token);
    }

    public async Task CommitTransaction(long transactionId)
    {
        await accountsRepositoryRefit.CommitTransaction(transactionId, config.Token);
    }

    public async Task CancelTransaction(long transactionId)
    {
        await accountsRepositoryRefit.CancelTransaction(transactionId, config.Token);
    }

    public async Task<bool> IsAccountOwner(long clientId, string accountNumber)
    {
        var accountSearchRequest = new IAccountsRepositoryRefit.AccountSearchRequest(clientId, MaxTake, 0);
        var response = await accountsRepositoryRefit.SearchAccounts(accountSearchRequest, config.Token);

        return response.Items.Any(item => item.Number == accountNumber && item.OwnerId == clientId);
    }
}
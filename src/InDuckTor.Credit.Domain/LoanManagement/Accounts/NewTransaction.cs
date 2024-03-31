namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public static class BankCodes
{
    public const string InDuckTorCode = "000000000";
}

public class NewTransaction
{
    public NewTransaction(decimal amount,
        TransactionAccountInfo? depositOn,
        TransactionAccountInfo? withdrawFrom,
        double? requestedTransactionTtl = null,
        bool executeImmediate = true)
    {
        Amount = amount;
        DepositOn = depositOn;
        WithdrawFrom = withdrawFrom;
        ExecuteImmediate = executeImmediate;
        RequestedTransactionTtl = requestedTransactionTtl;
    }

    public decimal Amount { get; set; }
    public TransactionAccountInfo? DepositOn { get; set; }
    public TransactionAccountInfo? WithdrawFrom { get; set; }
    public bool ExecuteImmediate { get; set; }
    public double? RequestedTransactionTtl { get; set; }

    public static NewTransaction ForInDuckTor(
        decimal amount,
        string depositOn,
        string withdrawFrom,
        double? requestedTransactionTtl = null,
        bool executeImmediate = true) => new(
        amount,
        TransactionAccountInfo.ForInDuckTor(depositOn),
        TransactionAccountInfo.ForInDuckTor(withdrawFrom),
        requestedTransactionTtl,
        executeImmediate);
}

public class TransactionAccountInfo
{
    public const string MasterAccount = "20202643900000000028";

    public TransactionAccountInfo(string accountNumber, string bankCode)
    {
        AccountNumber = accountNumber;
        BankCode = bankCode;
    }

    public string AccountNumber { get; set; }
    public string BankCode { get; set; }

    public static TransactionAccountInfo ForInDuckTor(string accountNumber) =>
        new(accountNumber, BankCodes.InDuckTorCode);

    public static TransactionAccountInfo ForMasterAccount() =>
        new(MasterAccount, BankCodes.InDuckTorCode);
}
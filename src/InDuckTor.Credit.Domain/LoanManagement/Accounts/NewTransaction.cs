namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public class NewTransaction
{
    public NewTransaction(decimal amount,
        TransactionAccountInfo? depositOn,
        TransactionAccountInfo? withdrawFrom,
        bool executeImmediate,
        double requestedTransactionTtl)
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
    public double RequestedTransactionTtl { get; set; }
}

public class TransactionAccountInfo
{
    public TransactionAccountInfo(string accountNumber, string bankCode)
    {
        AccountNumber = accountNumber;
        BankCode = bankCode;
    }

    public string AccountNumber { get; set; }
    public string BankCode { get; set; }
}
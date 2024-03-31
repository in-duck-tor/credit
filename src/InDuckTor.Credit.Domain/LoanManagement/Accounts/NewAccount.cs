using System.Runtime.Serialization;

namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public class NewAccount
{
    public NewAccount(long clientId, AccountType accountType, string currencyCode, DateTime plannedExpiration)
    {
        ClientId = clientId;
        AccountType = accountType;
        CurrencyCode = currencyCode;
        PlannedExpiration = plannedExpiration;
    }

    public long ClientId { get; set; }
    public AccountType AccountType { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime PlannedExpiration { get; set; }
}

public enum AccountState
{
    [EnumMember(Value = "active")] Active = 1,
    [EnumMember(Value = "closed")] Closed = 2,
    [EnumMember(Value = "frozen")] Frozen = 3
}

public enum AccountType
{
    /// <summary>
    /// Расчётный счёт
    /// </summary>
    [EnumMember(Value = "payment")] Payment = 1,

    /// <summary>
    /// Ссудный счёт
    /// </summary>
    [EnumMember(Value = "loan")] Loan = 2,

    /// <summary>
    /// Касса из которой ведется расчёт наличными; относится к счёту 20202 
    /// </summary>
    [EnumMember(Value = "cash_register")] CashRegister = 3,
}
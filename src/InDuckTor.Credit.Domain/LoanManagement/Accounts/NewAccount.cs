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

public enum AccountType
{
    /// <summary>
    /// Расчётный счёт
    /// </summary>
    [EnumMember(Value = "payment")] Payment,

    /// <summary>
    /// Ссудный счёт
    /// </summary>
    [EnumMember(Value = "loan")] Loan,
}
using System.Runtime.Serialization;

namespace InDuckTor.Credit.Domain.LoanManagement.Accounts;

public class TransactionInfo
{
    public TransactionInfo(long transactionId, TransactionType type, TransactionStatus status, DateTime autoCloseAt)
    {
        TransactionId = transactionId;
        Type = type;
        Status = status;
        AutoCloseAt = autoCloseAt;
    }

    public long TransactionId { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime AutoCloseAt { get; set; }
}

public enum TransactionType
{
    /// <summary>
    /// Снятие
    /// </summary>
    [EnumMember(Value = "withdraw")] Withdraw = 0,

    /// <summary>
    /// Внесение
    /// </summary>
    [EnumMember(Value = "deposit")] Deposit = 1,

    /// <summary>
    /// Перевод
    /// </summary>
    [EnumMember(Value = "transfer")] Transfer = 2,

    /// <summary>
    /// Перевод на внешний счёт
    /// </summary>
    [EnumMember(Value = "transfer_to_external")]
    TransferToExternal = 3,

    /// <summary>
    /// Перевод со внешнего счёт
    /// </summary>
    [EnumMember(Value = "transfer_from_external")]
    TransferFromExternal = 4
}

public enum TransactionStatus
{
    /// <summary>
    /// Трансакция в процессе создания 
    /// </summary>
    [EnumMember(Value = "creating")] Creating = 0,

    /// <summary>
    /// Трансакция обрабатывается
    /// </summary>
    [EnumMember(Value = "pending")] Pending = 1,

    /// <summary>
    /// Трансакция исполнена
    /// </summary>
    [EnumMember(Value = "committed")] Committed = 2,

    /// <summary>
    /// Трансакция отменена
    /// </summary>
    [EnumMember(Value = "canceled")] Canceled = 3,
}
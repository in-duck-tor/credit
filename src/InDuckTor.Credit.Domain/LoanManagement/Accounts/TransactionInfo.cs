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
    [EnumMember(Value = "withdraw")] Withdraw,

    /// <summary>
    /// Внесение
    /// </summary>
    [EnumMember(Value = "deposit")] Deposit,

    /// <summary>
    /// Перевод
    /// </summary>
    [EnumMember(Value = "transfer")] Transfer,

    /// <summary>
    /// Перевод на внешний счёт
    /// </summary>
    [EnumMember(Value = "transfer_to_external")]
    TransferToExternal,

    /// <summary>
    /// Перевод со внешнего счёт
    /// </summary>
    [EnumMember(Value = "transfer_from_external")]
    TransferFromExternal
}

public enum TransactionStatus
{
    /// <summary>
    /// Трансакция в процессе создания 
    /// </summary>
    [EnumMember(Value = "creating")] Creating,

    /// <summary>
    /// Трансакция обрабатывается
    /// </summary>
    [EnumMember(Value = "pending")] Pending,

    /// <summary>
    /// Трансакция исполнена
    /// </summary>
    [EnumMember(Value = "committed")] Committed,

    /// <summary>
    /// Трансакция отменена
    /// </summary>
    [EnumMember(Value = "canceled")] Canceled,
}
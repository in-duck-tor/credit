using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Financing.Program;

namespace InDuckTor.Credit.Domain.Financing.Application;

/// <summary>
/// <b>Заявка на Кредит</b>
/// </summary>
public class LoanApplication
{
    public const int MaxLoanSum = 5_000_000;
    
    public long Id { get; set; }

    public required long ClientId { get; set; }

    /// <summary>
    /// <b>Программа Кредитования</b>. Программа не меняется после того, как пользователь отправил заявку.
    /// Можно только отменить текущую заявку и отправить новую
    /// </summary>
    // public long LoanProgramId { get; set; }
    public required LoanProgram LoanProgram { get; init; }

    /// <summary>
    /// <b>Сумма Займа</b>
    /// </summary>
    public required decimal BorrowedAmount { get; set; }

    /// <summary>
    /// <b>Срок взятия кредита</b>
    /// </summary>
    public required TimeSpan LoanTerm { get; set; }

    /// <summary>
    /// <b>Счёт Клиента</b>
    /// </summary>
    public required string ClientAccountNumber { get; set; }

    public required ApplicationState ApplicationState { get; set; }

    /// <summary>
    /// <b>Дата одобрения заявки</b>
    /// </summary>
    public DateTime? ApprovalDate { get; set; }
}

/// <summary>
/// <b>Статус заявки</b>
/// </summary>
public enum ApplicationState
{
    [EnumMember(Value = "rejected")] Rejected,
    [EnumMember(Value = "pending")] Pending,
    [EnumMember(Value = "approved")] Approved,
    [EnumMember(Value = "processed")] Processed
}
using InDuckTor.Credit.Domain.Financing.Program;

namespace InDuckTor.Credit.Domain.Financing.Application;

/// <summary>
/// <b>Заявка на Кредит</b>
/// </summary>
public class LoanApplication
{
    public long Id { get; set; }

    public required long ClientId { get; set; }

    /// <summary>
    /// <b>Программа Кредитования</b>. Программа не меняется после того, как пользователь отправил заявку.
    /// Можно только отменить текущую заявку и отправить новую
    /// </summary>
    public required LoanProgram LoanProgram { get; init; }

    public required decimal BorrowedAmount { get; set; }

    /// <summary>
    /// <b>Срок взятия кредита</b>. Отсчёт от Рождения Христа
    /// </summary>
    public required DateTime LoanTerm { get; set; }

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
    Rejected,
    Pending,
    Approved
}
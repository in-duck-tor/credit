namespace InDuckTor.Credit.Domain.Loan.Financing;

/// <summary>
/// <b>Заявка на Кредит</b>
/// </summary>
public class LoanApplication
{
    public long Id { get; set; }

    /// <summary>
    /// <b>Программа Кредитования</b>. Программа не меняется после того, как пользователь отправил заявку.
    /// Можно только отменить текущую заявку и отправить новую
    /// </summary>
    public LoanProgram LoanProgram { get; init; }

    public decimal BorrowedAmount { get; set; }

    /// <summary>
    /// <b>Срок взятия кредита</b>
    /// </summary>
    public TimeSpan LoanTerm { get; set; }
}
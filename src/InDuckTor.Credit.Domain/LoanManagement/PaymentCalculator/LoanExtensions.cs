namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

internal static class LoanExtensions
{
    /// <summary>
    /// Возвращает дату начала и конца для нового периода, следующего за текущим
    /// </summary>
    /// <param name="loan">Кредит, для которого совершается операция</param>
    /// <returns></returns>
    internal static (DateTime startDate, DateTime endDate) GetNewPeriodDates(this Loan loan)
    {
        var now = DateTime.UtcNow;
        var startDate = loan.PeriodAccruals?.PeriodEndDate
                        ?? new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Kind);
        var endDate = now + loan.PeriodDuration;
        return (startDate, endDate);
    }
}
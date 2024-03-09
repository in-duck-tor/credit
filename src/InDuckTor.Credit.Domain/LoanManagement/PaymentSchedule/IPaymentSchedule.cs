namespace InDuckTor.Credit.Domain.LoanManagement.PaymentSchedule;

// В будущем через него можно будет строить предполагаемый график платежей
public interface IPaymentSchedule
{
    TimeSpan GetPeriodDuration(DateTime startDate);
    DateTime GetPeriodEndDate(DateTime startDate);
}

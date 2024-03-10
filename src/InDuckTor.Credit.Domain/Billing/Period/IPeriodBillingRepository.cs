namespace InDuckTor.Credit.Domain.Billing.Period;

public interface IPeriodBillingRepository
{
    List<PeriodBilling> GetAllUnpaidPeriodBillings(long loanId);
}
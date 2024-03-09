namespace InDuckTor.Credit.Domain.BillingPeriod;

public interface IPeriodBillingRepository
{
    List<PeriodBilling> GetAllUnpaidPeriodBillings(long loanId);
}
namespace InDuckTor.Credit.Domain.Billing.Period;

public interface IPeriodBillingRepository
{
    Task<List<PeriodBilling>> GetAllUnpaidPeriodBillings(long loanId);
}
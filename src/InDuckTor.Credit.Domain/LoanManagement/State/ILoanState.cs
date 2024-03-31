using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.LoanManagement.State;

public interface ILoanState
{
    void ChangeState(LoanState state);
    void StartNewPeriod();
    PeriodBilling ClosePeriod();
    decimal GetCurrentTotalPayment();
    decimal GetExpectedOneTimePayment();
    void AccrueInterestOnCurrentPeriod();
    decimal CalculateTickInterest();
    void ChargePenalty();
    void AttachLoanAccount(string accountNumber);
    bool IsCurrentPeriodEnded();
    void ActivateLoan();
    void CloseLoan();
    void SellToCollectors(int numberOfPeriods);
}
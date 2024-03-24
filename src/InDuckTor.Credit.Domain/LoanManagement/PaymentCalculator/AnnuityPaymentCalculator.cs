using InDuckTor.Credit.Domain.Billing;

namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

public class AnnuityPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();
    private readonly Loan _loan;

    public AnnuityPaymentCalculator(Loan loan)
    {
        _loan = loan;
    }

    public void StartNewPeriod()
    {
        var billing = _loan.LoanBilling;

        var now = DateTime.UtcNow;
        if (billing.PeriodAccruals != null && billing.PeriodAccruals.PeriodEndDate > now) return;

        var periodInterval = _loan.PeriodDuration();

        var startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Kind);
        var endDate = now + periodInterval;

        var monthlyInterestRate = (double)_loan.InterestRate / 12;
        var oneTimePayment = billing.LoanBody * (decimal)Math.Ceiling(
            monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, _loan.PlannedPaymentsNumber))
        );

        billing.PeriodAccruals = new PeriodAccruals
        {
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            OneTimePayment = oneTimePayment
        };
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        var billing = _loan.LoanBilling;
        ArgumentNullException.ThrowIfNull(billing.PeriodAccruals);

        var interest = _interestCalculator.InterestAccrual(_loan.LoanBilling.LoanBody, _loan.InterestRate);

        billing.PeriodAccruals.InterestAccrual += interest;
        billing.PeriodAccruals.LoanBodyPayoff =
            billing.PeriodAccruals.OneTimePayment - billing.PeriodAccruals.InterestAccrual;
    }
}
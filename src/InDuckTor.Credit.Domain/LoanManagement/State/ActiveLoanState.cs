using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Expenses;

namespace InDuckTor.Credit.Domain.LoanManagement.State;

public class ActiveLoanState : ILoanState
{
    public ActiveLoanState(Loan loan)
    {
        Loan = loan;
    }

    private const LoanState ThisState = LoanState.Active;
    private Loan Loan { get; }

    public void ChangeState(LoanState state)
    {
        switch (state)
        {
            case LoanState.Closed:
            case LoanState.Sold:
                Loan.SetState(state);
                break;
            case LoanState.Active:
                break;
            case LoanState.Approved:
                throw new Errors.Loan.InvalidLoanStateChange(
                    $"Cannot change state from {ThisState} to anything but {LoanState.Closed} or {LoanState.Sold}");
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void StartNewPeriod()
    {
        if (Loan.PeriodAccruals != null && !IsCurrentPeriodEnded())
            throw Errors.Loan.CannotStartNewPeriod.NotEndedYet();

        Loan.PaymentCalculator.StartNewPeriod();
    }

    public PeriodBilling ClosePeriod()
    {
        ArgumentNullException.ThrowIfNull(Loan.PeriodAccruals);

        Loan.PaymentCalculator.ClosePeriod();

        var billingItems = new ExpenseItems(
            Loan.PeriodAccruals.InterestAccrual,
            Loan.PeriodAccruals.LoanBodyPayoff,
            Loan.PeriodAccruals.ChargingForServices);

        var periodBilling = new PeriodBilling
        {
            Loan = Loan,
            PeriodStartDate = Loan.PeriodAccruals.PeriodStartDate,
            PeriodEndDate = Loan.PeriodAccruals.PeriodEndDate,
            OneTimePayment = Loan.PeriodAccruals.CurrentOneTimePayment,
            ExpenseItems = billingItems,
            RemainingPayoff = billingItems.DeepCopy(),
        };

        Loan.PeriodsBillings.Add(periodBilling);
        Loan.BodyAfterPayoffs.ChangeAmount(-periodBilling.ExpenseItems.LoanBodyPayoff);

        return periodBilling;
    }

    public decimal GetCurrentTotalPayment() => Loan.PaymentCalculator.GetCurrentTotalPayment();

    public decimal GetExpectedOneTimePayment() => Loan.PaymentCalculator.GetExpectedOneTimePayment();

    public void AccrueInterestOnCurrentPeriod() => Loan.PaymentCalculator.AccrueInterestOnCurrentPeriod();

    public decimal CalculateTickInterest() => Loan.CurrentBody * Loan.TickInterestRate;

    public void ChargePenalty()
    {
        Loan.Penalty.ChangeAmount(Loan.Debt * Loan.PenaltyRate);
    }

    public void AttachLoanAccount(string accountNumber)
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(AttachLoanAccount), ThisState);
    }

    public bool IsCurrentPeriodEnded()
    {
        // if (PaymentScheduleType == PaymentScheduleType.Calendar) return DateTime.UtcNow.Day == PeriodDay;
        ArgumentNullException.ThrowIfNull(Loan.PeriodAccruals);
        return Loan.PeriodAccruals.PeriodEndDate <= DateTime.UtcNow;
    }

    public void ActivateLoan()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(ActivateLoan), ThisState);
    }

    public void CloseLoan()
    {
        if (!Loan.IsRepaid)
            throw new Errors.Loan.InvalidLoanStateChange("Can't close the loan because it hasn't been repaid yet");

        ChangeState(LoanState.Closed);
    }

    public void SellToCollectors()
    {
        if (!Loan.IsClientAhuel)
            throw new Errors.Loan.InvalidLoanStateChange("Client is not ahuel enough to sell the loan to collectors");

        ChangeState(LoanState.Sold);
    }
}
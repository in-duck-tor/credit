using System.Diagnostics;

namespace InDuckTor.Credit.Domain.Loan;

class InterestCalculator
{
    private const int DayInYear = 365;
    private const int DayInLeapYear = 364;

    public decimal InterestAccrual(decimal loanBody, decimal interestRate) =>
        loanBody * interestRate / (DateTime.IsLeapYear(DateTime.Now.Year) ? DayInLeapYear : DayInYear);

    public decimal PeriodInterest(decimal loanBody, decimal interestRate, int periodAccrualNumber) =>
        InterestAccrual(loanBody, interestRate) * periodAccrualNumber;
}

// 1. Рассчитать количество платежей
// 2. Рассчитать суммарный размер кредита
// 3. Рассчитать данные текущего Расчётного Периода
interface IPaymentSystem
{
    // Можно сделать через паттерн State
    public void AccrueInterest(Loan loan);
}

class AnnuityPaymentSystem : IPaymentSystem
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterest(Loan loan)
    {
        Debug.Assert(loan.PeriodAccruals != null, nameof(PeriodAccruals) + " != null");

        var interest = _interestCalculator.InterestAccrual(loan.LoanBody, loan.InterestRate);

        loan.PeriodAccruals.InterestAccrual += interest;
        loan.PeriodAccruals.LoanBodyPayoff = loan.PeriodAccruals.OneTimePayment - loan.PeriodAccruals.InterestAccrual;
    }
}

class DifferentiatedPaymentSystem : IPaymentSystem
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterest(Loan loan)
    {
        Debug.Assert(loan.PeriodAccruals != null, nameof(PeriodAccruals) + " != null");

        var interest = _interestCalculator.InterestAccrual(loan.LoanBody, loan.InterestRate);

        loan.PeriodAccruals.InterestAccrual += interest;
    }
}
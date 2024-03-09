namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

class InterestCalculator
{
    private const int DayInYear = 365;
    private const int DayInLeapYear = 364;

    public decimal InterestAccrual(decimal loanBody, decimal interestRate) =>
        loanBody * interestRate / (DateTime.IsLeapYear(DateTime.Now.Year) ? DayInLeapYear : DayInYear);

    public decimal PeriodInterest(decimal loanBody, decimal interestRate, int periodAccrualNumber) =>
        InterestAccrual(loanBody, interestRate) * periodAccrualNumber;
}
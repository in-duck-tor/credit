namespace InDuckTor.Credit.Domain.BillingPeriod;

/// <summary>
/// <b>Статья расчёта</b>
/// </summary>
/// <param name="interest">Сумма процентов</param>
/// <param name="loanBodyPayoff">Остаток Платежа в пользу тела долга</param>
/// <param name="chargingForServices">Сумма стоимости дополнительных Услуг</param>
public class BillingItems(decimal interest, decimal loanBodyPayoff, decimal chargingForServices)
{
    public decimal Interest { get; set; } = interest;
    public decimal LoanBodyPayoff { get; set; } = loanBodyPayoff;
    public decimal ChargingForServices { get; set; } = chargingForServices;

    class PaymentCategory(decimal amount)
    {
        public decimal Amount { get; private set; } = amount;
        
        public void ChangeAmount(decimal amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
            Amount += amount;
        }
    }

    public void ChangeInterest(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Interest + amount);
        Interest += amount;
    }
    
    public void ChangeLoanBodyPayoff(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(LoanBodyPayoff + amount);
        LoanBodyPayoff += amount;
    }
    
    public void ChangeChargingForServices(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(ChargingForServices + amount);
        ChargingForServices += amount;
    }

    public decimal GetTotalSum() => Interest + LoanBodyPayoff + ChargingForServices;
    
    public BillingItems DeepCopy()
    {
        return new BillingItems(Interest, LoanBodyPayoff, ChargingForServices);
    }
}
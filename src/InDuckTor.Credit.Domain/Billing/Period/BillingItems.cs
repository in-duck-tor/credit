namespace InDuckTor.Credit.Domain.Billing.Period;

/// <summary>
/// <b>Статья расчёта</b>
/// </summary>
/// <param name="interest">Сумма процентов</param>
/// <param name="loanBodyPayoff">Остаток Платежа в пользу тела долга</param>
/// <param name="chargingForServices">Сумма стоимости дополнительных Услуг</param>
public class BillingItems(decimal interest, decimal loanBodyPayoff, decimal chargingForServices)
{
    public BillingItem Interest { get; private set; } = interest;
    public BillingItem LoanBodyPayoff { get; private set; } = loanBodyPayoff;
    public BillingItem ChargingForServices { get; private set; } = chargingForServices;

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
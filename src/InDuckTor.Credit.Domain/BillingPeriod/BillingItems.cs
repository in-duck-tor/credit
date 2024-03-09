using System.Reflection;

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

    public decimal GetTotalSum() => Interest + LoanBodyPayoff + ChargingForServices;
    
    public BillingItems DeepCopy()
    {
        return new BillingItems(Interest, LoanBodyPayoff, ChargingForServices);
    }
}
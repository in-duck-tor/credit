namespace InDuckTor.Credit.Domain.Loan.BillingPeriod;

/// <summary>
/// <b>Статья расчёта</b>
/// </summary>
/// <param name="Interest">Сумма процентов</param>
/// <param name="LoanBodyRepayment">Остаток Платежа в пользу тела долга</param>
/// <param name="ChargingForServices">Сумма стоимости дополнительных Услуг</param>
public class BillingItems(decimal Interest, decimal LoanBodyRepayment, decimal ChargingForServices);
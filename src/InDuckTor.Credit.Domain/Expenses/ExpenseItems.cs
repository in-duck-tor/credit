namespace InDuckTor.Credit.Domain.Expenses;

// todo: Переименовать сущность или сделать иерархию, чтобы были сущности статей расчёта, которые относятся только к периоду или ко всему кредиту в целом.
/// <summary>
/// <b>Статьи расчёта</b>
/// </summary>
/// <param name="interest">Сумма процентов</param>
/// <param name="loanBodyPayoff">Остаток Платежа в пользу тела долга</param>
/// <param name="chargingForServices">Сумма стоимости дополнительных Услуг</param>
public class ExpenseItems(decimal interest, decimal loanBodyPayoff, decimal chargingForServices)
{
    private ExpenseItems() : this(0, 0, 0)
    {
        // EF Core((((
    }
    
    public ExpenseItem Interest { get; private set; } = interest;
    public ExpenseItem LoanBodyPayoff { get; private set; } = loanBodyPayoff;
    public ExpenseItem ChargingForServices { get; private set; } = chargingForServices;

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

    public ExpenseItems DeepCopy()
    {
        return new ExpenseItems(Interest, LoanBodyPayoff, ChargingForServices);
    }
}
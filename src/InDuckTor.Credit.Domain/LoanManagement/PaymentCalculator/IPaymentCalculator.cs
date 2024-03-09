namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

// 1. Рассчитать количество платежей
// 2. Рассчитать суммарный размер кредита
// 3. Рассчитать данные текущего Расчётного Периода
public interface IPaymentCalculator
{
    // // Желательно отвязать от Loan, т.к. этот класс может использоваться без кредита. Например, для построения инфографики.
    void AccrueInterestOnCurrentPeriod(Loan loan);
}

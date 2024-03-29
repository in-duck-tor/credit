namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

// 1. Рассчитать количество платежей
// 2. Рассчитать суммарный размер кредита
// 3. Рассчитать данные текущего Расчётного Периода
public interface IPaymentCalculator
{
    void StartNewPeriod();
    void ClosePeriod();
    void AccrueInterestOnCurrentPeriod();

    decimal GetCurrentTotalPayment();
    decimal GetExpectedOneTimePayment();
    decimal GetExpectedBody();
    decimal GetExpectedInterest();
}
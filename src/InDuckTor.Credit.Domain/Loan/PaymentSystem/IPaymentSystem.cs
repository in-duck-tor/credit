using System.Diagnostics;

namespace InDuckTor.Credit.Domain.Loan.PaymentSystem;

// 1. Рассчитать количество платежей
// 2. Рассчитать суммарный размер кредита
// 3. Рассчитать данные текущего Расчётного Периода
interface IPaymentSystem
{
    // Можно сделать через паттерн State
    void AccrueInterest(Loan loan);
}

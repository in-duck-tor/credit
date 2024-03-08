using InDuckTor.Credit.Domain.Loan.Service.Model;

namespace InDuckTor.Credit.Domain.Loan.Service;

public class LoanService
{
    public Loan CreateLoan(NewLoan newLoan)
    {
        // newLoan.LoanTerm;

        var loan = new Loan
        {
            BorrowedAmount = newLoan.BorrowedAmount,
            InterestRate = newLoan.InterestRate,
            ApprovalDate = newLoan.ApprovalDate,
            PaymentType = newLoan.PaymentType,
            PaymentScheduleType = newLoan.PaymentScheduleType,
            State = LoanState.Approved
        };

        // 1. Присвоить поля из LoanInfo в соответствующие им поля из Loan
        // 2. Определить планируемое количетсво платежей
        // 3. Привязать кредит к расчётному счёту
        // 4. Создать ссудный счёт
        return default!;
    }
}
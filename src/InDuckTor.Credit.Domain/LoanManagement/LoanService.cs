using InDuckTor.Credit.Domain.LoanManagement.Interactor;
using InDuckTor.Credit.Domain.LoanManagement.Model;

namespace InDuckTor.Credit.Domain.LoanManagement;

public class LoanService(ILoanInteractorFactory loanInteractorFactory)
{
    public Loan CreateLoan(NewLoan newLoan)
    {
        // newLoan.LoanTerm;
        
        var loanInteractor = loanInteractorFactory.FromNewLoan(newLoan);
        
        // 1. Присвоить поля из LoanInfo в соответствующие им поля из Loan
        // 2. Определить планируемое количетсво платежей
        // 3. Привязать кредит к расчётному счёту
        // 4. Создать ссудный счёт
        
        
        return default!;
    }
}
using InDuckTor.Credit.Domain.Loan.Financing.Application.Model;
using InDuckTor.Credit.Domain.Loan.Service;
using InDuckTor.Credit.Domain.Loan.Service.Model;

namespace InDuckTor.Credit.Domain.Loan.Financing.Application;

public class ApplicationService(IApplicationRepository applicationRepository, LoanService loanService)
{
    public LoanApplication CreateApplication(NewApplication newApplication)
    {
        return default!;
        // var application = 
    }
    
    // Лучше крутить джобу, которая будет искать одобренные заявки и создавать кредиты, но так легче
    public void ApproveApplication(long applicationId)
    {
        var application = applicationRepository.GetApplicationById(applicationId);
        application.ApprovalDate = DateTime.Now;

        var newLoan = new NewLoan(
            application.BorrowedAmount,
            application.LoanProgram.InterestRate,
            application.ApprovalDate.GetValueOrDefault()
        );
        loanService.CreateLoan(newLoan);
    }

    public void CancelApplication(long applicationId)
    {
        var application = applicationRepository.GetApplicationById(applicationId);
        application.ApprovalDate = null;
    } 
}
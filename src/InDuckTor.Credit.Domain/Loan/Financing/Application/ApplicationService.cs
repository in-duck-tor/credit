using InDuckTor.Credit.Domain.Loan.Financing.Application.Model;
using InDuckTor.Credit.Domain.Loan.Financing.Program;
using InDuckTor.Credit.Domain.Loan.Service;
using InDuckTor.Credit.Domain.Loan.Service.Model;

namespace InDuckTor.Credit.Domain.Loan.Financing.Application;

public class ApplicationService(
    LoanService loanService,
    IApplicationRepository applicationRepository,
    ILoanProgramRepository loanProgramRepository)
{
    public LoanApplication CreateApplication(NewApplication newApplication)
    {
        // Сюда можно добавить валидацию заявки
        var loanProgram = loanProgramRepository.GetLoanProgramById(newApplication.LoanProgramId);
        return new LoanApplication
        {
            LoanProgram = loanProgram,
            BorrowedAmount = newApplication.BorrowedAmount,
            LoanTerm = newApplication.LoanTerm,
            ApplicationState = ApplicationState.Pending
        };
    }

    // Лучше крутить джобу, которая будет искать одобренные заявки и создавать кредиты, но так легче
    public void ApproveApplication(long applicationId)
    {
        var application = applicationRepository.GetApplicationById(applicationId);
        application.ApprovalDate = DateTime.Now;

        // var newLoan = new NewLoan(
        //     application.BorrowedAmount,
        //     application.LoanProgram.InterestRate,
        //     application.ApprovalDate.GetValueOrDefault()
        // );
        // loanService.CreateLoan(newLoan);
    }

    public void CancelApplication(long applicationId)
    {
        var application = applicationRepository.GetApplicationById(applicationId);
        application.ApprovalDate = null;
        application.ApplicationState = ApplicationState.Canceled;
    }
}
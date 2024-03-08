using InDuckTor.Credit.Domain.Loan.Financing.Application.Model;
using InDuckTor.Credit.Domain.Loan.Financing.Program;

namespace InDuckTor.Credit.Domain.Loan.Financing.Application;

public class ApplicationService(
    IApplicationRepository applicationRepository,
    ILoanProgramRepository loanProgramRepository)
{
    public LoanApplication CreateApplication(NewApplication newApplication)
    {
        // Сюда можно добавить валидацию заявки
        var loanProgram = loanProgramRepository.GetLoanProgramById(newApplication.LoanProgramId);
        return new LoanApplication
        {
            ClientId = newApplication.ClientId,
            LoanProgram = loanProgram,
            BorrowedAmount = newApplication.BorrowedAmount,
            LoanTerm = newApplication.LoanTerm,
            // Сейчас заявки только Approved, т.к. нет системы модерации заявок
            ApplicationState = ApplicationState.Approved
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

    public void RejectApplication(long applicationId)
    {
        var application = applicationRepository.GetApplicationById(applicationId);
        application.ApprovalDate = null;
        application.ApplicationState = ApplicationState.Rejected;
    }
}
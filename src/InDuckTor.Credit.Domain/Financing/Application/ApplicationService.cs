using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Financing.Application.Model;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Financing.Application;

public interface IApplicationService
{
    /// <summary>
    /// <para>Создаёт новую заявку на кредит.</para>
    /// <para>Сейчас создаётся уже одобренная заявка, т.к. отсутствует модерация.</para>
    /// </summary>
    /// <param name="newApplication"></param>
    /// <returns></returns>
    public Task<LoanApplication> CreateApplication(NewApplication newApplication);

    public Task ApproveApplication(long applicationId);
    public Task RejectApplication(long applicationId);
}

public class ApplicationService(
    IApplicationRepository applicationRepository,
    ILoanProgramRepository loanProgramRepository) : IApplicationService
{
    public async Task<LoanApplication> CreateApplication(NewApplication newApplication)
    {
        if (newApplication.BorrowedAmount > LoanApplication.MaxLoanSum)
            throw new Errors.LoanApplication.LoanSumIsTooBig();

        // Сюда можно добавить валидацию заявки
        var loanProgram = await loanProgramRepository.GetLoanProgramById(newApplication.LoanProgramId)
                          ?? throw new Errors.LoanProgram.NotFound(newApplication.LoanProgramId);

        // Расчёт реальной продолжительности Кредита, для предотвращения наеботеки
        var interestFreq = Loan.InterestAccrualFrequency;
        var realLoanTerm = interestFreq * Math.Round(newApplication.LoanTerm / interestFreq);

        return new LoanApplication
        {
            ClientId = newApplication.ClientId,
            LoanProgram = loanProgram,
            BorrowedAmount = newApplication.BorrowedAmount,
            LoanTerm = realLoanTerm,
            ClientAccountNumber = newApplication.ClientAccountNumber,
            // Сейчас заявки только Approved, т.к. нет системы модерации заявок
            ApplicationState = ApplicationState.Approved,
            ApprovalDate = DateTime.UtcNow
        };
    }

    // Лучше крутить джобу, которая будет искать одобренные заявки и создавать кредиты, но создать кредит сразу легче
    public async Task ApproveApplication(long applicationId)
    {
        var application = await applicationRepository.GetApplicationById(applicationId)
                          ?? throw new Errors.LoanApplication.NotFound(applicationId);
        application.ApprovalDate = DateTime.UtcNow;
    }

    public async Task RejectApplication(long applicationId)
    {
        var application = await applicationRepository.GetApplicationById(applicationId)
                          ?? throw new Errors.LoanApplication.NotFound(applicationId);
        application.ApprovalDate = null;
        application.ApplicationState = ApplicationState.Rejected;
    }
}
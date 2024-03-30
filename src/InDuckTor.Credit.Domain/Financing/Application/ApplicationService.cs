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
            throw new Errors.LoanApplication.LoanSumIsTooBig(LoanApplication.MaxLoanSum);

        // Сюда можно добавить валидацию заявки
        var loanProgram = await loanProgramRepository.GetLoanProgramById(newApplication.LoanProgramId)
                          ?? throw new Errors.LoanProgram.NotFound(newApplication.LoanProgramId);

        // Сделать продолжительность кредита кратной интервалам. Лучше выкидывать ошибку, но я забочусь о фронте
        var realLoanTerm = MakeMultiple(newApplication.LoanTerm, Loan.InterestAccrualFrequency);
        realLoanTerm = MakeMultiple(realLoanTerm, loanProgram.PeriodInterval!.Value);

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

    private TimeSpan MakeMultiple(TimeSpan changeable, TimeSpan multiple)
    {
        var round = Math.Round(changeable / multiple);
        if (round == 0) return multiple;
        return multiple * round;
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
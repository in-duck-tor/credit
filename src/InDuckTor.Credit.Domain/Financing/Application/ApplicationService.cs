using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Financing.Application.Extensions;
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
            throw Errors.LoanApplication.InvalidData.LoanSumIsTooBig(LoanApplication.MaxLoanSum);
        if (newApplication.LoanTerm <= TimeSpan.Zero) throw Errors.LoanApplication.InvalidData.LoanTerm();

        // Сюда можно добавить валидацию заявки
        var loanProgram = await loanProgramRepository.GetLoanProgramById(newApplication.LoanProgramId)
                          ?? throw new Errors.LoanProgram.NotFound(newApplication.LoanProgramId);

        // Сделать продолжительность кредита кратной интервалам. Лучше выкидывать ошибку, но я забочусь о фронте
        var realLoanTerm = newApplication.LoanTerm
            .MultipleOf(Loan.InterestAccrualFrequency)
            .MultipleOf(loanProgram.PeriodInterval!.Value);

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
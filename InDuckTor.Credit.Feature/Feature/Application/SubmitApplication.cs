using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Application.Model;
using InDuckTor.Credit.Feature.Feature.Interceptors;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Application;

/// <param name="ClientId">Id Клиента</param>
/// <param name="LoanProgramId">Id Программы Кредитования</param>
/// <param name="BorrowedAmount">Сумма займа</param>
/// <param name="LoanTerm">Время, на которое берётся Кредит в секундах</param>
public record ApplicationInfo(
    long ClientId,
    long LoanProgramId,
    decimal BorrowedAmount,
    long LoanTerm,
    string ClientAccountNumber);

public record LoanApplicationResponse(
    long Id,
    long ClientId,
    LoanProgramResponse LoanProgram,
    decimal BorrowedAmount,
    TimeSpan LoanTerm,
    ApplicationState ApplicationState,
    // todo: разделить на несколько запросов
    LoanInfoResponse Loan)
{
    public static LoanApplicationResponse
        FromApplication(LoanApplication application, Domain.LoanManagement.Loan loan) => new(
        application.Id,
        application.ClientId,
        LoanProgramResponse.FromLoanProgram(application.LoanProgram),
        application.BorrowedAmount,
        application.LoanTerm,
        application.ApplicationState,
        LoanInfoResponse.FromLoan(loan)
    );
}

public interface ISubmitApplication : ICommand<ApplicationInfo, LoanApplicationResponse>;

[Intercept(typeof(SaveChangesInterceptor<ApplicationInfo, LoanApplicationResponse>))]
public class SubmitApplication(
    LoanDbContext context,
    IApplicationService applicationService,
    IExecutor<ICreateLoan, LoanApplication, Domain.LoanManagement.Loan> createLoan) : ISubmitApplication
{
    public async Task<LoanApplicationResponse> Execute(ApplicationInfo info, CancellationToken ct)
    {
        var newApplication = FromInfo(info);

        var application = await applicationService.CreateApplication(newApplication);
        context.LoanApplications.Add(application);

        // todo: Создаётся сразу, т.к. у нас нет модерации. В будущем надо создавать либо через джобу, либо сразу после одобрения
        var loan = await createLoan.Execute(application, ct);

        return LoanApplicationResponse.FromApplication(application, loan);
    }

    private static NewApplication FromInfo(ApplicationInfo info) => new()
    {
        ClientId = info.ClientId,
        LoanProgramId = info.LoanProgramId,
        BorrowedAmount = info.BorrowedAmount,
        LoanTerm = TimeSpan.FromSeconds(info.LoanTerm),
        ClientAccountNumber = info.ClientAccountNumber
    };

    private void EnqueueTransferMoneyJob(Domain.LoanManagement.Loan loan)
    {
        // BackgroundJob.Enqueue();
    }
}
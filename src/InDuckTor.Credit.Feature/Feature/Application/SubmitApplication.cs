using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Application.Model;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Application;

/// <param name="ClientId">Id Клиента</param>
/// <param name="LoanProgramId">Id Программы Кредитования</param>
/// <param name="BorrowedAmount">Сумма займа</param>
/// <param name="LoanTerm">Время, на которое берётся Кредит в секундах</param>
public record ApplicationInfoRequest(
    long ClientId,
    long LoanProgramId,
    decimal BorrowedAmount,
    long LoanTerm,
    string ClientAccountNumber);

/// <param name="Id">Id Заявки</param>
/// <param name="ClientId">Id Клиента</param>
/// <param name="LoanProgram">Время, на которое берётся Кредит в секундах</param>
/// <param name="BorrowedAmount">Сумма займа</param>
/// <param name="LoanTerm">Время, на которое берётся Кредит в секундах</param>
/// <param name="ApplicationState">Состояние Заявки</param>
/// <param name="Loan">Созданный Кредит (если будет модерация, то этого поля не будет)</param>
public record LoanApplicationResponse(
    long Id,
    long ClientId,
    LoanProgramResponse LoanProgram,
    MoneyView BorrowedAmount,
    long LoanTerm,
    ApplicationState ApplicationState,
    // todo: разделить на несколько запросов
    LoanInfoResponse Loan)
{
    public static LoanApplicationResponse FromApplication(
        LoanApplication application,
        Domain.LoanManagement.Loan loan) => new(
        application.Id,
        application.ClientId,
        LoanProgramResponse.FromLoanProgram(application.LoanProgram),
        application.BorrowedAmount,
        (long)application.LoanTerm.TotalSeconds,
        application.ApplicationState,
        LoanInfoResponse.FromLoan(loan)
    );
}

public interface ISubmitApplication : ICommand<ApplicationInfoRequest, LoanApplicationResponse>;

public class SubmitApplication(
    LoanDbContext context,
    IApplicationService applicationService,
    IExecutor<ICreateLoan, LoanApplication, Domain.LoanManagement.Loan> createLoan) : ISubmitApplication
{
    public async Task<LoanApplicationResponse> Execute(ApplicationInfoRequest infoRequest, CancellationToken ct)
    {
        var newApplication = FromInfo(infoRequest);

        var application = await applicationService.CreateApplication(newApplication);
        context.LoanApplications.Add(application);

        // todo: Создаётся сразу, т.к. у нас нет модерации. В будущем надо создавать либо через джобу, либо сразу после одобрения
        var loan = await createLoan.Execute(application, ct);

        await context.SaveChangesAsync(ct);

        return LoanApplicationResponse.FromApplication(application, loan);
    }

    private static NewApplication FromInfo(ApplicationInfoRequest infoRequest) => new()
    {
        ClientId = infoRequest.ClientId,
        LoanProgramId = infoRequest.LoanProgramId,
        BorrowedAmount = infoRequest.BorrowedAmount,
        LoanTerm = TimeSpan.FromSeconds(infoRequest.LoanTerm),
        ClientAccountNumber = infoRequest.ClientAccountNumber
    };
}
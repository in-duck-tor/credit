using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Application.Model;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Application;

public record ApplicationInfo(long ClientId, long LoanProgramId, decimal BorrowedAmount, TimeSpan LoanTerm);

// "clientId": 0,
// "loanProgramId": 0,
// "borrowedAmount": 0,
// "loanTerm": "2024-03-10T13:28:13.602Z"

public interface ISubmitApplication : ICommand<ApplicationInfo, long>;

public class SubmitApplication(ApplicationService applicationService) : ISubmitApplication
{
    public Task<long> Execute(ApplicationInfo input, CancellationToken ct)
    {
        var newApplication = new NewApplication
        {
            ClientId = input.ClientId,
            LoanProgramId = input.LoanProgramId,
            BorrowedAmount = input.BorrowedAmount,
            LoanTerm = input.LoanTerm
        };
        return Task.FromResult(applicationService.CreateApplication(newApplication).Id);
    }
}
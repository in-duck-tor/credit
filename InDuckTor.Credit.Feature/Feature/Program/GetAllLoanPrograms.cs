using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Program;

public record LoanProgramShortResponse(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);

public interface IGetAllLoanPrograms : IQuery<Unit, LoanProgramShortResponse>;

public class GetAllLoanPrograms : IGetAllLoanPrograms
{
    public Task<LoanProgramShortResponse> Execute(Unit input, CancellationToken ct)
    {
        // todo:
        throw new NotImplementedException();
    }
}
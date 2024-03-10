using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Program;

public record LoanProgramInfo(
    decimal InterestRate,
    PaymentType PaymentType,
    PaymentScheduleType PaymentScheduleType);

// todo: возвращать dto, а не доменную сущность
public interface ICreateLoanProgram : IQuery<LoanProgramInfo, LoanProgram>;

public class CreateLoanProgram(LoanProgramService loanProgramService) : ICreateLoanProgram
{
    public Task<LoanProgram> Execute(LoanProgramInfo input, CancellationToken ct)
    {
        return Task.FromResult(loanProgramService.CreateProgram(new(input.InterestRate, input.PaymentType,
            input.PaymentScheduleType)));
    }
}
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Financing.Application.Extensions;
using InDuckTor.Credit.Domain.Financing.Program.Model;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Financing.Program;

public interface ILoanProgramService
{
    LoanProgram CreateProgram(NewProgram newProgram);
}

public class LoanProgramService : ILoanProgramService
{
    public LoanProgram CreateProgram(NewProgram newProgram)
    {
        if (newProgram.PeriodInterval <= TimeSpan.Zero) throw Errors.LoanProgram.InvalidData.Interval();
        if (newProgram.InterestRate <= 0) throw Errors.LoanProgram.InvalidData.Interest();

        var realPeriodInterval = newProgram.PeriodInterval.MultipleOf(Loan.InterestAccrualFrequency);

        return new LoanProgram
        {
            InterestRate = newProgram.InterestRate,
            PaymentType = newProgram.PaymentType,
            PaymentScheduleType = newProgram.PaymentScheduleType,
            PeriodInterval = realPeriodInterval
        };
    }
}
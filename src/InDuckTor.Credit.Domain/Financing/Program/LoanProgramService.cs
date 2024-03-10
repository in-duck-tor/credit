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
        var interestFreq = Loan.InterestAccrualFrequency;
        var realPeriodInterval = interestFreq * Math.Round(newProgram.PeriodInterval / interestFreq);

        return new LoanProgram
        {
            InterestRate = newProgram.InterestRate,
            PaymentType = newProgram.PaymentType,
            PaymentScheduleType = newProgram.PaymentScheduleType,
            PeriodInterval = realPeriodInterval
        };
    }
}
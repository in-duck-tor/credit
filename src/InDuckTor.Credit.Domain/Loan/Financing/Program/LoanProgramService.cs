using InDuckTor.Credit.Domain.Loan.Financing.Program.Model;

namespace InDuckTor.Credit.Domain.Loan.Financing.Program;

public class LoanProgramService
{
    public LoanProgram CreateProgram(NewProgram newProgram)
    {
        return new LoanProgram
        {
            InterestRate = newProgram.InterestRate,
            PaymentType = newProgram.PaymentType,
            PaymentScheduleType = newProgram.PaymentScheduleType
        };
    }
}
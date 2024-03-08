namespace InDuckTor.Credit.Domain.Loan.Financing.Program;

public interface ILoanProgramRepository
{
    LoanProgram GetLoanProgramById(long id);
}
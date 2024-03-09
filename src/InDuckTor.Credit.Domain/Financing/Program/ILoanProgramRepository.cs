namespace InDuckTor.Credit.Domain.Financing.Program;

public interface ILoanProgramRepository
{
    LoanProgram GetLoanProgramById(long id);
}
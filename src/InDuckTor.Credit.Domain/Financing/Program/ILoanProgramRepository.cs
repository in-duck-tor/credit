namespace InDuckTor.Credit.Domain.Financing.Program;

public interface ILoanProgramRepository
{
    Task<LoanProgram?> GetLoanProgramById(long id);
}
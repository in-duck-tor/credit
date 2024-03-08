namespace InDuckTor.Credit.Domain.Loan.Financing.Application;

public interface IApplicationRepository
{
    LoanApplication GetApplicationById(long id);
    
}
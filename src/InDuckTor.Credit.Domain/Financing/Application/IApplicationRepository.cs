namespace InDuckTor.Credit.Domain.Financing.Application;

public interface IApplicationRepository
{
    LoanApplication GetApplicationById(long id);
    
}
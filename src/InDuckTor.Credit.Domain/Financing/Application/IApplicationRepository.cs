namespace InDuckTor.Credit.Domain.Financing.Application;

public interface IApplicationRepository
{
    Task<LoanApplication?> GetApplicationById(long id);
    
}
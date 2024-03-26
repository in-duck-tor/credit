using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.LoanManagement.Models;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Loan;

// Сделать это как background job. Нужно собрать все апрувнутые кредиты и создать их (потом скорее всего нужно будет отрефакторить)
public interface ICreateLoan : ICommand<LoanApplication, Domain.LoanManagement.Loan>;

public class CreateLoan(
    LoanDbContext context,
    ILoanService loanService) : ICreateLoan
{
    public async Task<Domain.LoanManagement.Loan> Execute(LoanApplication loanApplication, CancellationToken ct)
    {
        var loan = await loanService.CreateLoan(NewLoan.FromApplication(loanApplication));
        context.Loans.Add(loan);

        loanApplication.ApplicationState = ApplicationState.Processed;

        return await Task.FromResult(loan);
    }
}
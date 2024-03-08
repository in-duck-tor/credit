namespace InDuckTor.Credit.Domain.Loan;

public record NewLoan(decimal BorrowedAmount, decimal InterestRate, DateTime ApprovalDate);

public static class LoanInfoExtensions
{
    public static Loan ToDomain(this NewLoan newLoan) => new()
    {
        BorrowedAmount = newLoan.BorrowedAmount,
        InterestRate = newLoan.InterestRate,
        ApprovalDate = newLoan.ApprovalDate
    };
}

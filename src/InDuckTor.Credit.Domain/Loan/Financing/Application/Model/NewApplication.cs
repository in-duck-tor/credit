namespace InDuckTor.Credit.Domain.Loan.Financing.Application.Model;

public class NewApplication
{
    public long LoanProgramId { get; set; }
    public decimal BorrowedAmount { get; set; }
    public DateTime LoanTerm { get; set; }
}
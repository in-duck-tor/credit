namespace InDuckTor.Credit.Domain.Financing.Application.Model;

public class NewApplication
{
    public long ClientId { get; set; }
    public long LoanProgramId { get; set; }
    public decimal BorrowedAmount { get; set; }
    public DateTime LoanTerm { get; set; }
}
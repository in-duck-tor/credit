namespace InDuckTor.Credit.Domain.Financing.Application.Model;

public class NewApplication
{
    public required long ClientId { get; set; }
    public required long LoanProgramId { get; set; }
    public required decimal BorrowedAmount { get; set; }
    public required TimeSpan LoanTerm { get; set; }
    public required string ClientAccountNumber { get; set; }
}
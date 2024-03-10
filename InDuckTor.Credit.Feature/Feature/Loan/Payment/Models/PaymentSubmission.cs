namespace InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;

public record PaymentSubmission(long LoanId, long ClientId, long Payment);
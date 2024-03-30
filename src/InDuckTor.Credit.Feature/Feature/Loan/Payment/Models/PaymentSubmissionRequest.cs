namespace InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;

public record PaymentSubmissionRequest(long LoanId, long ClientId, decimal Payment);
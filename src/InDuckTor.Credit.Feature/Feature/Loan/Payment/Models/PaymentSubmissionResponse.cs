namespace InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;

public record PaymentSubmissionResponse(
    long LoanId,
    long ClientId,
    MoneyView PaymentAmount);
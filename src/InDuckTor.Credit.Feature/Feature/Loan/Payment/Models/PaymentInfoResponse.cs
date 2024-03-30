namespace InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;

public record PaymentInfoResponse(
    long LoanId,
    long ClientId,
    MoneyView PaymentAmount);
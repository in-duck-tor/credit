namespace InDuckTor.Credit.Domain.Billing.Payment.Models;

public record NewPayment(long LoanId, long ClientId, decimal PaymentAmount);
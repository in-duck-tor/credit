using System.ComponentModel.DataAnnotations;

namespace InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;

public record PaymentSubmissionRequest(
    [property: Required] long LoanId,
    [property: Required] long ClientId,
    [property: Required] decimal Payment);
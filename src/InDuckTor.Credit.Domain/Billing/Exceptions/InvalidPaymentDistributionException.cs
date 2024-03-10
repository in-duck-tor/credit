using InDuckTor.Credit.Domain.Exceptions;

namespace InDuckTor.Credit.Domain.Billing.Exceptions;

public class InvalidPaymentDistributionException(string message) : DomainException(message);
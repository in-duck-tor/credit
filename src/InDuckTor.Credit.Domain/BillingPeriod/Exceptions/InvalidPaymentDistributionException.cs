using InDuckTor.Credit.Domain.Exceptions;

namespace InDuckTor.Credit.Domain.BillingPeriod.Exceptions;

public class InvalidPaymentDistributionException(string message) : DomainException(message);
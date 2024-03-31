using InDuckTor.Credit.Domain.LoanManagement.State;

namespace InDuckTor.Credit.Domain.Exceptions;

public static class Errors
{
    public abstract class BusinessLogicException(string? message = null) : Exception(message);

    public abstract class ForbiddenError(string? message = null) : Exception(message);

    public abstract class InternalError(string? message = null) : Exception(message);

    public class EntitiesIsNotRelatedException(string? message = null) : BusinessLogicException(message)
    {
        public static EntitiesIsNotRelatedException WithNames(string entity1Name, string entity2Name)
        {
            return new EntitiesIsNotRelatedException(
                $"Entity with name '{entity1Name}' has no relation with entity with name '{entity2Name}'");
        }
    }

    public class NotFoundException(string message) : Exception(message);

    public class BadRequestException(string message) : Exception(message);

    public static class Transaction
    {
        public static class CannotInitiateTransaction
        {
            public class ClientIsNotAccountOwner(
                string message = "Cannot initiate transaction: user is not account owner") : ForbiddenError(message);

            public class Unknown(string message = "Cannot initiate transaction: something went wrong")
                : InternalError(message);
        }
    }

    public static class Loan
    {
        public class NotFound(string message) : NotFoundException(message)
        {
            public NotFound(long id) : this($"Loan with id '{id}' is not found")
            {
            }

            public NotFound(long clientId, long loanId) : this(
                $"Loan with id {loanId} for client with id {clientId} is not found")
            {
            }
        }

        public class CannotStartNewPeriod(string message) : BusinessLogicException(message)
        {
            public static CannotStartNewPeriod NotEndedYet() => new(
                "Cannot start a new period because the current one has not ended yet");
        }

        public class InvalidLoanStateChange(string message) : BusinessLogicException(message);

        public static class InvalidLoanState
        {
            public class Forbidden(string message) : ForbiddenError(message)
            {
                public Forbidden(long loanId, string action, LoanState state) : this(
                    $"Cannot perform action '{action}': the loan with id '{loanId}' is {state.ToString()}")
                {
                }
            }
        }
    }

    public static class LoanApplication
    {
        public class NotFound(long id) : NotFoundException($"LoanApplication with id '{id}' is not found");

        public class InvalidData(string message) : BadRequestException(message)
        {
            public static InvalidData LoanTerm() => new("Loan term must be positive");
            public static InvalidData LoanSumIsTooBig(decimal maxAmount) =>
                new($"The amount borrowed by client is too large. The max loan amount is {maxAmount}");
        }
    }

    public static class LoanProgram
    {
        public class NotFound(string message) : NotFoundException(message)
        {
            public NotFound(long id) : this($"LoanProgram with id '{id}' is not found")
            {
            }
        }

        public class InvalidData(string message) : BadRequestException(message)
        {
            public static InvalidData Interval() => new InvalidData("Interval must be positive");
            public static InvalidData Interest() => new InvalidData("Interest must be positive");
        }
    }

    public static class Payment
    {
        public class InvalidRegularPaymentAmount(string message) : BadRequestException(message)
        {
            public static InvalidRegularPaymentAmount TooMuch() =>
                new("Payments exceed the amount of debts and regular payments");

            public static void ThrowIfNotPositive(decimal amount)
            {
                if (amount <= 0) throw new InvalidRegularPaymentAmount("The payment amount must be positive");
            }
        }

        public class InvalidPaymentDistributionException(string message) : BusinessLogicException(message);
    }

    public static class CreditScore
    {
        public class NotFound(string message) : NotFoundException(message)
        {
            public NotFound(long id) : this($"CreditScore for client with id '{id}' is not found")
            {
            }
        }
    }
}
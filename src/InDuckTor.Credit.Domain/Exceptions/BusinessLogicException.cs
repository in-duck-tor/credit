using InDuckTor.Credit.Domain.LoanManagement.State;

namespace InDuckTor.Credit.Domain.Exceptions;

public static class Errors
{
    public abstract class BusinessLogicException(string? message = null) : Exception(message);

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

    public static class Loan
    {
        public class NotFound(string message) : NotFoundException(message)
        {
            public NotFound(long id) : this($"Loan with id '{id}' is not found")
            {
            }
        }

        public class CannotStartNewPeriod(string message) : BusinessLogicException(message)
        {
            public static CannotStartNewPeriod NotEndedYet() => new(
                "Cannot start a new period because the current one has not ended yet");
        }

        public class CannotProvideLoan(string message = "Cannot provide a loan to a client")
            : BusinessLogicException(message);

        public class InvalidLoanStateChange(string message) : BusinessLogicException(message);

        public class InvalidLoanState(string message) : BusinessLogicException(message)
        {
            public InvalidLoanState(long loanId, string action, LoanState state) : this(
                $"Cannot perform action '{action}': the loan with id '{loanId}' is {state.ToString()}")
            {
            }

            public static InvalidLoanState Closed(long loanId) =>
                new($"Cannot perform action: the loan with id '{loanId}' is closed");
        }
    }

    public static class LoanApplication
    {
        public class NotFound(long id) : NotFoundException($"LoanApplication with id '{id}' is not found");

        public class LoanSumIsTooBig(decimal maxAmount) : BadRequestException(
            $"The amount borrowed by client is too large. The max loan amount is {maxAmount}");
    }

    public static class LoanProgram
    {
        public class NotFound(string message) : NotFoundException(message)
        {
            public NotFound(long id) : this($"LoanProgram with id '{id}' is not found")
            {
            }
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
}
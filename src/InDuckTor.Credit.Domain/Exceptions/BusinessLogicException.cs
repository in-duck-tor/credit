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

    public class NotFound(string message) : Exception(message);

    public static class Loan
    {
        public class NotFound(string message) : Errors.NotFound(message)
        {
            public NotFound(long id) : this($"Loan with id '{id}' is not found")
            {
            }
        }

        public class InvalidLoanStateChange(string message) : BusinessLogicException(message);
    }

    public static class LoanApplication
    {
        public class NotFound(long id) : Errors.NotFound($"LoanApplication with id '{id}' is not found");
    }

    public static class LoanProgram
    {
        public class NotFound(string message) : Errors.NotFound(message)
        {
            public NotFound(long id) : this($"LoanProgram with id '{id}' is not found")
            {
            }
        }
    }

    public static class Payment
    {
        public class InvalidRegularPaymentAmount(string message) : BusinessLogicException(message)
        {
            public static InvalidRegularPaymentAmount TooMuch() =>
                new("Payments exceed the amount of debts and regular payments");
        }

        public class InvalidPaymentDistributionException(string message) : BusinessLogicException(message);
    }
}
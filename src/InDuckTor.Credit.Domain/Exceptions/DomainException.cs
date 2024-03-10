namespace InDuckTor.Credit.Domain.Exceptions;

public abstract class DomainException(string? message = null) : Exception(message);

public class EntitiesIsNotRelatedException(string? message = null) : DomainException(message)
{
    public static EntitiesIsNotRelatedException WithNames(string entity1Name, string entity2Name)
    {
        return new EntitiesIsNotRelatedException(
            $"Entity with name '{entity1Name}' has no relation with entity with name '{entity2Name}'");
    }
}
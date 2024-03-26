using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Expenses;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InDuckTor.Credit.Infrastructure.Config.Database.Converters;

public class BillingItemConverter : ValueConverter<ExpenseItem, decimal>
{
    public BillingItemConverter() : base(
        number => number.Amount,
        amount => new ExpenseItem(amount))
    {
    }
}
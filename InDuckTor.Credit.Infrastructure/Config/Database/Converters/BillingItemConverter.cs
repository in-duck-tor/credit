using InDuckTor.Credit.Domain.Billing.Period;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InDuckTor.Credit.Infrastructure.Config.Database.Converters;

public class BillingItemConverter : ValueConverter<BillingItem, decimal>
{
    public BillingItemConverter() : base(
        number => number.Amount,
        amount => new BillingItem(amount))
    {
    }
}
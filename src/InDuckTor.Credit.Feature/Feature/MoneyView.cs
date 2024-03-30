using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using InDuckTor.Credit.Domain.Expenses;

namespace InDuckTor.Credit.Feature.Feature;

// Эта тема не должна быть общей, она должна быть привязана к конкретной валюте (т.к. scale у битка выше, чем у физических валют)
[JsonConverter(typeof(MoneyViewJsonConverter))]
public class MoneyView
{
    private const int Scale = 2;

    public MoneyView(decimal amount)
    {
        Amount = Math.Round(amount, Scale, MidpointRounding.ToPositiveInfinity);
    }

    public decimal Amount { get; }

    public static implicit operator decimal(MoneyView mv) => mv.Amount;
    public static implicit operator MoneyView(decimal amount) => new(amount);
    public static implicit operator MoneyView(ExpenseItem ei) => new(ei);
}

public class MoneyViewJsonConverter : JsonConverter<MoneyView>
{
    public override MoneyView Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!decimal.TryParse(reader.GetString(), CultureInfo.InvariantCulture, out var number))
            throw new ArgumentException("Error trying to parse string to MoneyView");

        return number;
    }

    public override void Write(Utf8JsonWriter writer, MoneyView value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Amount);
    }
}
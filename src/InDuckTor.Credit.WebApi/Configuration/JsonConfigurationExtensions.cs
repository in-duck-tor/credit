using System.Text.Json.Serialization;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.LoanManagement;
using Microsoft.AspNetCore.Http.Json;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class JsonConfigurationExtensions
{
    public static IServiceCollection ConfigureJsonConverters(this IServiceCollection serviceCollection) => serviceCollection.Configure<JsonOptions>(ConfigureJsonOptions);

    private static void ConfigureJsonOptions(JsonOptions options)
    {
        var enumMemberConverter = new JsonStringEnumMemberConverter(
            new JsonStringEnumMemberConverterOptions(),
            typeof(ApplicationState), typeof(LoanState), typeof(PaymentType), typeof(PaymentScheduleType));

        options.SerializerOptions.Converters.Add(enumMemberConverter);
    }
}
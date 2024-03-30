using System.Xml.Linq;
using System.Xml.XPath;
using InDuckTor.Credit.WebApi.Configuration.Exceptions;
using InDuckTor.Shared.Configuration.Swagger;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCreditSwaggerGen(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSwaggerGen(options =>
        {
            options.ConfigureJwtAuth();
            options.ConfigureEnumMemberValues();
            options.CustomSchemaIds(ComposeNameWithDeclaringType);

            options.DocumentFilter<CustomModelDocumentFilter<ErrorResponse>>();

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            foreach (var fi in dir.EnumerateFiles("*.xml"))
            {
                var doc = XDocument.Load(fi.FullName);
                options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
            }
        });
    }

    private static string ComposeNameWithDeclaringType(Type type) =>
        type.DeclaringType is null
            ? type.Name
            : string.Join('.', ComposeNameWithDeclaringType(type.DeclaringType), type.Name);
}

public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
    }
}
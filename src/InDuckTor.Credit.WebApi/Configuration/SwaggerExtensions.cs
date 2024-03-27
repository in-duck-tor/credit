using System.Xml.Linq;
using System.Xml.XPath;
using InDuckTor.Shared.Configuration.Swagger;

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

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            foreach (var fi in dir.EnumerateFiles("*.xml"))
            {
                var doc = XDocument.Load(fi.FullName);
                options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
            }
        });

        string ComposeNameWithDeclaringType(Type type)
            => type.DeclaringType is null
                ? type.Name
                : string.Join('.', ComposeNameWithDeclaringType(type.DeclaringType), type.Name);
    }
}
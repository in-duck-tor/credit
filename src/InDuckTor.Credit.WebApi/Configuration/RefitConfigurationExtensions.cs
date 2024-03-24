using System.Net.Http.Headers;
using InDuckTor.Credit.Feature.Repository;
using InDuckTor.Credit.WebApi.Configuration.Models;
using Refit;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class RefitConfigurationExtensions
{
    public static IServiceCollection ConfigureRefit(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("AccountsSettings").Get<AccountsSettings>();
        ArgumentNullException.ThrowIfNull(settings);

        var httpClientBuilder = serviceCollection
            .AddRefitClient<IAccountsRepositoryRefit>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            })
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(settings.BaseAddress))
            .AddHttpMessageHandler(serviceProvider =>
                new HttpLoggingHandler(serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()));

        httpClientBuilder.Services.AddSingleton<HttpLoggingHandler>();

        return serviceCollection;
    }
}

public class HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id}: Request]";

        logger.LogInformation(
            $"""
             {msg}
             =========Start========="
             Method: {request.Method}
             Path: {request.RequestUri?.PathAndQuery} {request.RequestUri?.Scheme}/{request.Version}
             Host: {request.RequestUri?.Scheme}://{request.RequestUri?.Host}
             """);

        foreach (var header in request.Headers)
            logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (request.Content != null)
        {
            foreach (var header in request.Content.Headers)
                logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (request.Content is StringContent || IsTextBasedContentType(request.Headers) ||
                IsTextBasedContentType(request.Content.Headers))
            {
                var result = await request.Content.ReadAsStringAsync(cancellationToken);

                logger.LogInformation($"""
                                       {msg} Content:
                                       {result}
                                       """);
            }
        }

        var start = DateTime.Now;
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var end = DateTime.Now;

        logger.LogInformation($"""
                               {msg} Duration: {end - start}
                               ==========End==========
                               """);

        msg = $"[{id} - Response]";
        logger.LogInformation(
            $"""
             {msg}=========Start=========
             {msg} {request.RequestUri?.Scheme.ToUpper()}/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}
             """);

        foreach (var header in response.Headers)
            logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        foreach (var header in response.Content.Headers)
            logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (response.Content is StringContent || IsTextBasedContentType(response.Headers) ||
            IsTextBasedContentType(response.Content.Headers))
        {
            start = DateTime.Now;
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            end = DateTime.Now;

            logger.LogInformation($"""
                                   {msg} Content:
                                   {msg} {result}
                                   {msg} Duration: {end - start}
                                   """);
        }

        logger.LogInformation($"{msg}==========End==========");
        return response;
    }

    private readonly string[] _types = ["html", "text", "xml", "json", "txt", "x-www-form-urlencoded"];

    private bool IsTextBasedContentType(HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out var values))
            return false;
        var header = string.Join(" ", values).ToLowerInvariant();

        return _types.Any(t => header.Contains(t));
    }
}
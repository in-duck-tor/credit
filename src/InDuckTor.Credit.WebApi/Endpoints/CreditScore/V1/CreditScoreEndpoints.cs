using InDuckTor.Credit.Feature.Feature.CreditScore;
using InDuckTor.Shared.Security.Context;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.CreditScore.V1;

public static class Endpoints
{
    public static IEndpointRouteBuilder AddCreditScoreEndpointsV1(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/score")
            .WithTags(SwaggerTags.CreditScore)
            .WithOpenApi();

        groupBuilder.MapGet("/", GetCreditScore)
            .RequireAuthorization()
            .WithSummary("Получение информации о кредитном рейтинге");

        return builder;
    }

    private static async Task<IResult> GetCreditScore(
        ISecurityContext securityContext,
        [FromServices] IExecutor<IGetCreditScore, long, CreditScoreResponse> getCreditScore,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var смородина = securityContext.Currant;
        var result = await getCreditScore.Execute(смородина.Id, cancellationToken);
        return TypedResults.Ok(result);
    }
}
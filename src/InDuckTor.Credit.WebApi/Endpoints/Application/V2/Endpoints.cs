using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.WebApi.Endpoints.Application.V2.Contracts.Body;
using InDuckTor.Credit.WebApi.Endpoints.Idempotency;
using InDuckTor.Shared.Idempotency.Http;
using InDuckTor.Shared.Security.Context;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.Application.V2;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanApplicationEndpointsV2(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v2/application")
            .WithTags(SwaggerTags.ApplicationV2)
            .WithOpenApi()
            .WithIdempotencyKey(ttlSeconds: IdempotencyUtils.TimeToLive);

        groupBuilder.MapPost("", CreateApplication)
            .WithSummary("Создаёт заявку на получение кредита. С авторизацией пользователя через токен")
            .RequireAuthorization();

        return builder;
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<LoanApplicationResponse>(200)]
    private static async Task<IResult> CreateApplication(
        ISecurityContext securityContext,
        [FromBody] CreateApplicationBody body,
        [FromServices] IExecutor<ISubmitApplication, ApplicationInfoRequest, LoanApplicationResponse> submitApplication,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var userContext = securityContext.Currant;

        var request = new ApplicationInfoRequest(
            userContext.Id,
            body.LoanProgramId,
            body.BorrowedAmount,
            body.LoanTerm,
            body.ClientAccountNumber);

        var result = await submitApplication.Execute(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
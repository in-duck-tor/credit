using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.WebApi.Endpoints.Idempotency;
using InDuckTor.Shared.Idempotency.Http;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.Application.V1;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanApplicationEndpointsV1(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/application")
            .WithTags(SwaggerTags.ApplicationV1)
            .WithOpenApi()
            .WithIdempotencyKey(ttlSeconds: IdempotencyUtils.TimeToLive);

        groupBuilder.MapPost("", CreateApplication)
            .WithSummary("Создаёт заявку на получение кредита");

        groupBuilder.MapPost("/approve", ApproveApplication)
            .WithSummary("Одобрение заявки на получение кредита")
            .WithOpenApi(o =>
            {
                o.Deprecated = true;
                return o;
            });

        groupBuilder.MapPost("/reject", RejectApplication)
            .WithSummary("Отклонение заявки на получение кредита")
            .WithOpenApi(o =>
            {
                o.Deprecated = true;
                return o;
            });

        return builder;
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<LoanApplicationResponse>(200)]
    private static async Task<IResult> CreateApplication(
        [FromBody] ApplicationInfoRequest body,
        [FromServices] IExecutor<ISubmitApplication, ApplicationInfoRequest, LoanApplicationResponse> submitApplication,
        CancellationToken cancellationToken)
    {
        var result = await submitApplication.Execute(body, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static Ok ApproveApplication([FromQuery] long applicationId)
    {
        throw new NotImplementedException();
    }

    private static Ok RejectApplication([FromQuery] long applicationId)
    {
        throw new NotImplementedException();
    }
}
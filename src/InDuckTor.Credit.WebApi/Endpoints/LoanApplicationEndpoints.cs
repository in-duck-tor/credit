using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.WebApi.Configuration.Exceptions;
using InDuckTor.Credit.WebApi.Contracts.Bodies;
using InDuckTor.Shared.Security.Context;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace InDuckTor.Credit.WebApi.Endpoints;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder AddLoanApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.ConfigureV1Endpoints()
            .ConfigureV2Endpoints();
    }

    private static IEndpointRouteBuilder ConfigureV2Endpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v2/application")
            .WithTags(SwaggerTags.ApplicationV2)
            .WithOpenApi();

        groupBuilder.MapPost("", CreateApplicationV2)
            .WithDescription("Создаёт заявку на получение кредита. С авторизацией пользователя через токен")
            .RequireAuthorization();

        return builder;
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<LoanApplicationResponse>(200)]
    private static async Task<IResult> CreateApplicationV2(
        ISecurityContext securityContext,
        [FromBody] ApplicationInfoBodyV2 body,
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

    private static IEndpointRouteBuilder ConfigureV1Endpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/application")
            .WithTags(SwaggerTags.ApplicationV1)
            .WithOpenApi();

        groupBuilder.MapPost("", CreateApplicationV1)
            .WithDescription("Создаёт заявку на получение кредита");

        groupBuilder.MapPost("/approve", ApproveApplication)
            .WithDescription("Одобрение заявки на получение кредита")
            .WithOpenApi(o =>
            {
                o.Deprecated = true;
                return o;
            });

        groupBuilder.MapPost("/reject", RejectApplication)
            .WithDescription("Отклонение заявки на получение кредита")
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
    private static async Task<IResult> CreateApplicationV1(
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
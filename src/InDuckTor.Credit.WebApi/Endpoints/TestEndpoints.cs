using Hangfire;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InDuckTor.Credit.WebApi.Endpoints;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder AddTestEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/test")
            .WithTags(SwaggerTags.Test)
            .WithOpenApi();

        groupBuilder.MapPost("/tick", TriggerTick)
            .WithSummary("Вызвать начисление процентов по кредитам, а также закрытие расчётных периодов/кредитов");

        return builder;
    }

    private static NoContent TriggerTick()
    {
        BackgroundJob.Enqueue((IExecutor<ILoanInterestTick, Unit, Unit> loanInterestTick) =>
            loanInterestTick.Execute(default, default));
        return TypedResults.NoContent();
    }
 }
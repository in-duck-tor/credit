using InDuckTor.Credit.Feature.Feature.Program;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.WebApi.Endpoints.Idempotency;
using InDuckTor.Shared.Idempotency.Http;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.Program.V1;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanProgramEndpointsV1(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/program")
            .WithTags(SwaggerTags.ProgramV1)
            .WithOpenApi()
            .WithIdempotencyKey(ttlSeconds: IdempotencyUtils.TimeToLive);

        groupBuilder.MapGet("", GetAllLoanPrograms)
            .WithSummary("Получение всех программ кредитования");

        groupBuilder.MapPost("", CreateLoanProgram)
            .WithSummary("Создание программы кредитования");

        return builder;
    }

    // В будущем добавить эндпонит на получение доступных только клиенту программ кредитования
    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<List<LoanProgramResponse>>(200)]
    private static async Task<IResult> GetAllLoanPrograms(
        [FromServices] IExecutor<IGetAllLoanPrograms, Unit, List<LoanProgramResponse>> getAllLoanPrograms,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok(await getAllLoanPrograms.Execute(default, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<LoanProgramResponse>(200)]
    private static async Task<IResult> CreateLoanProgram(
        [FromBody] LoanProgramInfoRequest body,
        [FromServices] IExecutor<ICreateLoanProgram, LoanProgramInfoRequest, LoanProgramResponse> createLoanProgram,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok(await createLoanProgram.Execute(body, cancellationToken));
    }
}
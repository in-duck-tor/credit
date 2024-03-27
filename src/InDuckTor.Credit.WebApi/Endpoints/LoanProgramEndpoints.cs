using InDuckTor.Credit.Feature.Feature.Program;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints;

public static class LoanProgramEndpoints
{
    public static IEndpointRouteBuilder AddLoanProgramEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/program")
            .WithTags("Program")
            .WithOpenApi();

        groupBuilder.MapGet("", GetAllLoanPrograms)
            .WithDescription("Получение всех программ кредитования");

        groupBuilder.MapPost("", CreateLoanProgram)
            .WithDescription("Создание программы кредитования");

        return builder;
    }

    // В будущем добавить эндпонит на получение доступных только клиенту программ кредитования
    [ProducesResponseType<List<LoanProgramResponse>>(200)]
    private static async Task<IResult> GetAllLoanPrograms(
        [FromServices] IExecutor<IGetAllLoanPrograms, Unit, List<LoanProgramResponse>> getAllLoanPrograms,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok(await getAllLoanPrograms.Execute(default, cancellationToken));
    }

    // Здесь должна быть проверка прав вызывающего
    [ProducesResponseType<LoanProgramResponse>(200)]
    private static async Task<IResult> CreateLoanProgram(
        [FromBody] LoanProgramInfo body,
        [FromServices] IExecutor<ICreateLoanProgram, LoanProgramInfo, LoanProgramResponse> createLoanProgram,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok(await createLoanProgram.Execute(body, cancellationToken));
    }
}
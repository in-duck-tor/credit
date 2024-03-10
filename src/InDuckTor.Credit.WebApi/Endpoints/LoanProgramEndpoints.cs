using InDuckTor.Credit.Feature.Feature.Common;
using InDuckTor.Credit.Feature.Feature.Program;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.WebApi.Contracts.Responses;
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
    private static async Task<Ok<List<LoanProgramResponse>>> GetAllLoanPrograms(
        [FromServices] IExecutor<IGetAllLoanPrograms, Unit, List<LoanProgramResponse>> getAllLoanPrograms,
        CancellationToken cancellationToken
    )
    {
        var result = await getAllLoanPrograms.Execute(default, cancellationToken);
        return TypedResults.Ok(result);
    }

    // Здесь должна быть проверка прав вызывающего
    private static async Task<Ok<LoanProgramResponse>> CreateLoanProgram(
        [FromBody] LoanProgramInfo body,
        [FromServices] IExecutor<ICreateLoanProgram, LoanProgramInfo, LoanProgramResponse> createLoanProgram,
        CancellationToken cancellationToken
    )
    {
        var result = await createLoanProgram.Execute(body, cancellationToken);
        return TypedResults.Ok(result);
    }
}
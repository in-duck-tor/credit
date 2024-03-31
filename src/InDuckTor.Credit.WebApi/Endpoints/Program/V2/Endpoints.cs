using InDuckTor.Credit.Feature.Feature.Program;
using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.Program.V2;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanProgramEndpointsV2(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v2/program")
            .WithTags(SwaggerTags.ProgramV2)
            .WithOpenApi();
        
        groupBuilder.MapPost("", CreateLoanProgram)
            .WithSummary("Создание программы кредитования")
            .WithDescription("Доступно только сотрудникам");

        return builder;
    }
    
    [Authorize(Policy = "EmployeeOnly")]
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
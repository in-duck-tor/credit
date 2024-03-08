using InDuckTor.Credit.WebApi.Contracts.Responses;
using Microsoft.AspNetCore.Http.HttpResults;

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
    private static Ok<List<LoanProgramShortResponse>> GetAllLoanPrograms()
    {
        throw new NotImplementedException();
    }
    
    // Здесь должна быть проверка прав вызывающего
    private static Ok<LoanProgramShortResponse> CreateLoanProgram()
    {
        throw new NotImplementedException();
    }
}
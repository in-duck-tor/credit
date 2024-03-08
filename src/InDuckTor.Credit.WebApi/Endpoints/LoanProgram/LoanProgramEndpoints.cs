using InDuckTor.Credit.WebApi.Endpoints.LoanApplication.Models.Request;
using InDuckTor.Credit.WebApi.Endpoints.LoanProgram.Model.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.LoanProgram;

public static class LoanProgramEndpoints
{
    public static IEndpointRouteBuilder AddLoanProgramEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1")
            .WithTags("LoanApplication")
            .WithOpenApi();
        
        groupBuilder.MapGet("/program", GetAllLoanPrograms)
            .WithDescription("Получение всех программ кредитования");
        
        groupBuilder.MapPost("/program", CreateLoanProgram)
            .WithDescription("Создание программы кредитования");

        return builder;
    }

    // В будущем добавить эндпонит на получение доступных только клиенту программ
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
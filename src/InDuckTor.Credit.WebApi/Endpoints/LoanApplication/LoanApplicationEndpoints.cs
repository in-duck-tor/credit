using InDuckTor.Credit.WebApi.Endpoints.LoanApplication.Models.Request;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.LoanApplication;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder AddLoanApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1")
            .WithTags("LoanApplication")
            .WithOpenApi();
        
        groupBuilder.MapPost("/application", CreateApplication)
            .WithDescription("Создаёт заявку на получение кредита");

        groupBuilder.MapPost("/application/approve", ApproveApplication)
            .WithDescription("Одобрение заявки на получение кредита");
        
        groupBuilder.MapPost("/application/reject", RejectApplication)
            .WithDescription("Отклонение заявки на получение кредита");

        return builder;
    }

    private static Ok<CreateLoanApplicationRequest> CreateApplication(
        [FromBody] CreateLoanApplicationRequest request)
    {
        throw new NotImplementedException();
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
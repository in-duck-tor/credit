using InDuckTor.Credit.WebApi.Contracts.Bodies;
using InDuckTor.Credit.WebApi.Contracts.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder AddLoanApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/application")
            .WithTags("LoanApplication")
            .WithOpenApi();

        groupBuilder.MapPost("", CreateApplication)
            .WithDescription("Создаёт заявку на получение кредита")
            .WithOpenApi(o =>
            {
                o.Deprecated = true;
                return o;
            });


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

    private static Ok<LoanApplicationResponse> CreateApplication(
        [FromBody] CreateLoanApplicationBody body)
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
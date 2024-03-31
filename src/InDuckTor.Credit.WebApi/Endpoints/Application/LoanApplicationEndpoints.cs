using InDuckTor.Credit.WebApi.Endpoints.Application.V1;
using InDuckTor.Credit.WebApi.Endpoints.Application.V2;

namespace InDuckTor.Credit.WebApi.Endpoints.Application;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder AddLoanApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.AddLoanApplicationEndpointsV1()
            .AddLoanApplicationEndpointsV2();
    }
}
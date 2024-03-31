using InDuckTor.Credit.WebApi.Endpoints.Program.V1;
using InDuckTor.Credit.WebApi.Endpoints.Program.V2;

namespace InDuckTor.Credit.WebApi.Endpoints.Program;

public static class LoanProgramEndpoints
{
    public static IEndpointRouteBuilder AddLoanProgramEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.AddLoanProgramEndpointsV1()
            .AddLoanProgramEndpointsV2();
    }
}
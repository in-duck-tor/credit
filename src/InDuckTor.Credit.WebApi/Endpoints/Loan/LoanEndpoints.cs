using InDuckTor.Credit.WebApi.Endpoints.Loan.V1;
using InDuckTor.Credit.WebApi.Endpoints.Loan.V2;

namespace InDuckTor.Credit.WebApi.Endpoints.Loan;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder AddLoanEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .AddLoanEndpointsV1()
            .AddLoanEndpointsV2();
    }
}
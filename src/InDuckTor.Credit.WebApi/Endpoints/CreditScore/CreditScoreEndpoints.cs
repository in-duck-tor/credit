using InDuckTor.Credit.WebApi.Endpoints.CreditScore.V1;

namespace InDuckTor.Credit.WebApi.Endpoints.CreditScore;

public static class CreditScoreEndpoints
{
    public static IEndpointRouteBuilder AddCreditScoreEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.AddCreditScoreEndpointsV1();
    }
}
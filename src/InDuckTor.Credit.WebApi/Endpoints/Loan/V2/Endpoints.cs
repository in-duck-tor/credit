using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Feature.Feature.Loan.Payment;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.WebApi.Endpoints.Loan.V2.Contracts.Body;
using InDuckTor.Shared.Security.Context;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Mvc;
using EarlyPayoffBody = InDuckTor.Credit.WebApi.Endpoints.Loan.V1.Contracts.Body.EarlyPayoffBody;
using PaymentInfoResponse = InDuckTor.Credit.Feature.Feature.Loan.Payment.PaymentInfoResponse;

namespace InDuckTor.Credit.WebApi.Endpoints.Loan.V2;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanEndpointsV2(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v2/loan")
            .WithTags(SwaggerTags.LoanV2)
            .WithOpenApi()
            .RequireAuthorization();

        groupBuilder.MapPost("/{loanId:long}/pay/regularly", PayRegularly)
            .WithSummary("Внесение средств для оплаты кредита в регулярном порядке. С авторизацей по токену")
            .WithDescription("Если платёж клиента превысит сумму задолженностей и " +
                             "платежа по текущему расчётному периоду, операция отклонится");
        groupBuilder.MapPost("/{loanId:long}/pay/early", PayEarly)
            .WithSummary("Внесение средств для досрочной оплаты кредита")
            .WithDescription("Досрочно можно оплачивать только если успешно внесена " +
                             "оплата за текущий расчётный период. Иначе операция отклонится")
            .WithOpenApi(o =>
            {
                o.Deprecated = true;
                return o;
            });

        groupBuilder.MapGet("/{loanId:long}", GetLoanInfoForClient)
            .WithSummary("Получение клиентом информации о конкретном кредите. С авторизацей по токену");

        groupBuilder.MapGet("/{loanId:long}/overdue", GetOverduePeriods)
            .WithSummary("Получение клиентом информации о просроченных платёжных периодах. С авторизацей по токену");

        groupBuilder.MapGet("/{loanId:long}/paymentinfo", GetPaymentInfo)
            .WithSummary("Получение информации о платежу по кредиту. С авторизацей по токену");

        groupBuilder.MapGet("/loans", GetAllClientLoans)
            .WithSummary("Получение информации обо всех кредитах пользователя. С авторизацей по токену");

        return builder;
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<PaymentInfoResponse>(200)]
    private static async Task<IResult> GetPaymentInfo(
        ISecurityContext securityContext,
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetPaymentInfoV2, GetPaymentInfoRequestV2, PaymentInfoResponse> getPeriodPaymentInfo,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var clientId = securityContext.Currant.Id;

        var request = new GetPaymentInfoRequestV2(LoanId: loanId, ClientId: clientId);
        return TypedResults.Ok(await getPeriodPaymentInfo.Execute(request, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<List<PeriodInfoResponse>>(200)]
    private static async Task<IResult> GetOverduePeriods(
        ISecurityContext securityContext,
        [FromRoute] long loanId,
        [FromServices]
        IExecutor<IGetOverduePeriodsV2, GetOverduePeriodsRequest, List<PeriodInfoResponse>> getOverduePeriods,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var clientId = securityContext.Currant.Id;

        var request = new GetOverduePeriodsRequest(LoanId: loanId, ClientId: clientId);
        return TypedResults.Ok(await getOverduePeriods.Execute(request, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType(204)]
    private static async Task<IResult> PayRegularly(
        ISecurityContext securityContext,
        [FromRoute] long loanId,
        [FromBody] RegularPayBodyV2 body,
        [FromServices]
        IExecutor<ISubmitRegularPayment, PaymentSubmissionRequest, PaymentSubmissionResponse> submitRegularPayment,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var clientId = securityContext.Currant.Id;

        var submission = new PaymentSubmissionRequest(LoanId: loanId, ClientId: clientId, body.Payment);
        await submitRegularPayment.Execute(submission, cancellationToken);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType(204)]
    private static Task<IResult> PayEarly(
        [FromRoute] long loanId,
        [FromBody] EarlyPayoffBody body,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<LoanInfoResponse>(200)]
    private static async Task<IResult> GetLoanInfoForClient(
        ISecurityContext securityContext,
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetLoanInfoV2, GetLoanInfoRequestV2, LoanInfoResponse> getLoanInfo,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var clientId = securityContext.Currant.Id;

        var request = new GetLoanInfoRequestV2(clientId, loanId);
        return TypedResults.Ok(await getLoanInfo.Execute(request, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<List<LoanInfoShortResponse>>(200)]
    private static async Task<IResult> GetAllClientLoans(
        ISecurityContext securityContext,
        [FromServices] IExecutor<IGetAllClientLoans, long, List<LoanInfoShortResponse>> getAllClientLoans,
        CancellationToken cancellationToken)
    {
        if (!securityContext.IsImpersonated) return TypedResults.Unauthorized();
        var clientId = securityContext.Currant.Id;

        return TypedResults.Ok(await getAllClientLoans.Execute(clientId, cancellationToken));
    }
}
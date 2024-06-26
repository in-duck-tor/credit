using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Feature.Feature.Loan.Payment;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.WebApi.Endpoints.Idempotency;
using InDuckTor.Credit.WebApi.Endpoints.Loan.V1.Contracts.Body;
using InDuckTor.Shared.Idempotency.Http;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints.Loan.V1;

public static class Endpoints
{
    internal static IEndpointRouteBuilder AddLoanEndpointsV1(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/loan")
            .WithTags(SwaggerTags.LoanV1)
            .WithOpenApi()
            .WithIdempotencyKey(ttlSeconds: IdempotencyUtils.TimeToLive);

        groupBuilder.MapPost("/{loanId:long}/pay/regularly", PayRegularly)
            .WithSummary("Внесение средств для оплаты кредита в регулярном порядке")
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
            .WithSummary("Получение клиентом информации о конкретном кредите");

        groupBuilder.MapGet("/{loanId:long}/overdue", GetOverduePeriods)
            .WithSummary("Получение клиентом информации о просроченных платёжных периодах");

        groupBuilder.MapGet("/{loanId:long}/paymentinfo", GetPaymentInfo)
            .WithSummary("Получение информации о платежу по кредиту");

        groupBuilder.MapGet("/client/{clientId:long}", GetAllClientLoans)
            .WithSummary("Получение информации обо всех кредитах пользователя");

        return builder;
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<PaymentInfoResponse>(200)]
    private static async Task<IResult> GetPaymentInfo(
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetPaymentInfoV1, long, PaymentInfoResponse> getPeriodPaymentInfo,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getPeriodPaymentInfo.Execute(loanId, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType<List<PeriodInfoResponse>>(200)]
    private static async Task<IResult> GetOverduePeriods(
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetOverduePeriodsV1, long, List<PeriodInfoResponse>> getOverduePeriods,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getOverduePeriods.Execute(loanId, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(403)]
    [ProducesResponseType<ProblemDetails>(401)]
    [ProducesResponseType(204)]
    private static async Task<IResult> PayRegularly(
        [FromRoute] long loanId,
        [FromBody] RegularPayBodyV1 body,
        [FromServices]
        IExecutor<ISubmitRegularPayment, PaymentSubmissionRequest, PaymentSubmissionResponse> submitRegularPayment,
        CancellationToken cancellationToken)
    {
        var submission = new PaymentSubmissionRequest(loanId, body.ClientId, body.Payment);
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
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetLoanInfoV1, long, LoanInfoResponse> getLoanInfo,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getLoanInfo.Execute(loanId, cancellationToken));
    }

    [ProducesResponseType<ProblemDetails>(500)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<List<LoanInfoShortResponse>>(200)]
    private static async Task<IResult> GetAllClientLoans(
        [FromRoute] long clientId,
        [FromServices] IExecutor<IGetAllClientLoans, long, List<LoanInfoShortResponse>> getAllClientLoans,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getAllClientLoans.Execute(clientId, cancellationToken));
    }
}
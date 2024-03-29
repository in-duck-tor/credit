using Hangfire;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Feature.Feature.Loan.Payment;
using InDuckTor.Credit.Feature.Feature.Loan.Payment.Models;
using InDuckTor.Credit.WebApi.Contracts.Bodies;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InDuckTor.Credit.WebApi.Endpoints;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder AddLoanEndpoints(this IEndpointRouteBuilder builder)
    {
        var groupBuilder = builder.MapGroup("/api/v1/loan")
            .WithTags("Loan")
            .WithOpenApi();

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

        groupBuilder.MapGet("/client/{clientId:long}", GetAllClientLoans)
            .WithSummary("Получение информации обо всех кредитах пользователя");

        groupBuilder.MapPost("/tick", TriggerTick)
            .WithSummary("Вызвать начисление процентов по кредитам, а также закрытие расчётных периодов/кредитов");

        return builder;
    }

    private static NoContent TriggerTick()
    {
        BackgroundJob.Enqueue((IExecutor<ILoanInterestTick, Unit, Unit> loanInterestTick) =>
            loanInterestTick.Execute(default, default));
        return TypedResults.NoContent();
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    [ProducesResponseType(401)]
    [ProducesResponseType(204)]
    private static async Task<IResult> PayRegularly(
        [FromRoute] long loanId,
        [FromBody] RegularPayBody body,
        [FromServices] IExecutor<ISubmitRegularPayment, PaymentSubmission, Unit> submitRegularPayment,
        CancellationToken cancellationToken)
    {
        var submission = new PaymentSubmission(loanId, body.ClientId, body.Payment);
        await submitRegularPayment.Execute(submission, cancellationToken);
        return TypedResults.NoContent();
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    [ProducesResponseType(401)]
    [ProducesResponseType(204)]
    private static Task<IResult> PayEarly(
        [FromRoute] long loanId,
        [FromBody] EarlyPayoffBody body,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    [ProducesResponseType<LoanInfoResponse>(200)]
    private static async Task<IResult> GetLoanInfoForClient(
        [FromRoute] long loanId,
        [FromServices] IExecutor<IGetLoanInfo, long, LoanInfoResponse> getLoanInfo,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getLoanInfo.Execute(loanId, cancellationToken));
    }

    [ProducesResponseType(404)]
    [ProducesResponseType<List<LoanInfoShortResponse>>(200)]
    private static async Task<IResult> GetAllClientLoans(
        [FromRoute] long clientId,
        [FromServices] IExecutor<IGetAllClientLoans, long, List<LoanInfoShortResponse>> getAllClientLoans,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await getAllClientLoans.Execute(clientId, cancellationToken));
    }
}
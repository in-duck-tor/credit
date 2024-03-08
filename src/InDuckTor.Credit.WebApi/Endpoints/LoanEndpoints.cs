using InDuckTor.Credit.WebApi.Contracts.Bodies;
using InDuckTor.Credit.WebApi.Contracts.Responses;
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

        // 1. Оплата текущего расчётного периода вместе с задолжностями и штрафами
        // 2. Досрочное погашение
        // 3. Получение размера платежа для погашения текущего расчётного периода вместе с задолжностями (сущность, с категориями)
        // 4. Получение информации о кредите

        groupBuilder.MapPost("/{loanId:long}/pay/regularly", PayRegularly)
            .WithSummary("Внесение средств для оплаты кредита в регулярном порядке")
            .WithDescription("Если платёж клиента превысит сумму задолженностей и " +
                             "платежа по текущему расчётному периоду, операция отклонится");

        groupBuilder.MapPost("/{loanId:long}/pay/early", PayoffEarly)
            .WithSummary("Внесение средств для досрочной оплаты кредита")
            .WithDescription("Досрочно можно оплачивать только если успешно внесена " +
                             "оплата за текущий расчётный период. Иначе операция отклонится");

        groupBuilder.MapGet("/{loanId:long}", GetLoanInfoForClient)
            .WithSummary("Получение клиентом информации о конкретном кредите");

        groupBuilder.MapGet("/client/{clientId:long}", GetAllClientLoans)
            .WithSummary("Получение информации обо всех кредитах пользователя");

        return builder;
    }

    private static Results<Ok, BadRequest, ForbidHttpResult> PayRegularly(
        [FromRoute] long loanId,
        [FromBody] RegularPayBody body)
    {
        throw new NotImplementedException();
    }

    private static Results<Ok, BadRequest, ForbidHttpResult> PayoffEarly(
        [FromRoute] long loanId,
        [FromBody] EarlyPayoffBody body)
    {
        throw new NotImplementedException();
    }

    private static Ok<LoanInfoResponse> GetLoanInfoForClient([FromRoute] long loanId)

    {
        throw new NotImplementedException();
    }

    private static Ok<CreateLoanApplicationBody> GetAllClientLoans(
        [FromRoute] long clientId,
        [FromBody] CreateLoanApplicationBody body)

    {
        throw new NotImplementedException();
    }
}
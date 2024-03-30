using System.Net;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.WebApi.Configuration.Models;
using Microsoft.AspNetCore.Diagnostics;
using Refit;

namespace InDuckTor.Credit.WebApi.Configuration.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError("An error occurred while processing your request: {message}", exception.Message);

        var er = new ErrorResponse { Message = exception.Message };

        switch (exception)
        {
            case Errors.NotFoundException:
                er.StatusCode = (int)HttpStatusCode.NotFound;
                er.SetType(exception);
                er.Title = "Not Found Exception";
                break;
            case Errors.BadRequestException:
            case BadHttpRequestException:
                er.StatusCode = (int)HttpStatusCode.BadRequest;
                er.SetType(exception);
                er.Title = "Bad Request Exception";
                break;
            case Errors.BusinessLogicException:
                er.StatusCode = (int)HttpStatusCode.InternalServerError;
                er.SetType(exception);
                er.Title = "Business Logic Error. Pizda.";
                break;
            case ApiException apiException:
                var apiErrorResponse = await apiException.GetContentAsAsync<ApiExceptionResponse>();
                er.StatusCode = apiErrorResponse?.Status ?? (int)apiException.StatusCode;
                er.SetType(apiException);
                er.Title = apiErrorResponse?.Title ?? apiException.ReasonPhrase ?? er.Type;
                er.Message = apiErrorResponse?.Detail ?? apiException.Content ?? er.Message;
                break;
            default:
                er.StatusCode = (int)HttpStatusCode.InternalServerError;
                er.SetType(exception);
                er.Title = "Internal Server Error";
                break;
        }

        httpContext.Response.StatusCode = er.StatusCode;

        await httpContext.Response.WriteAsJsonAsync(er, cancellationToken);

        return true;
    }
}
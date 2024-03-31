using System.Net;
using InDuckTor.Credit.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Refit;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

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

        var er = new ProblemDetails { Detail = exception.Message };

        switch (exception)
        {
            case Errors.NotFoundException:
                er.Status = (int)HttpStatusCode.NotFound;
                er.Type = exception.GetType().Name;
                er.Title = "Not Found Exception";
                break;
            case Errors.BadRequestException:
            case BadHttpRequestException:
                er.Status = (int)HttpStatusCode.BadRequest;
                er.Type = exception.GetType().Name;
                er.Title = "Bad Request Exception";
                break;
            case Errors.BusinessLogicException:
                er.Status = (int)HttpStatusCode.InternalServerError;
                er.Type = exception.GetType().Name;
                er.Title = "Business Logic Error. Pizda.";
                break;
            case ApiException apiException:
                Console.WriteLine(apiException.Content);
                // var apiErrorResponse = await apiException.GetContentAsAsync<ProblemDetails>();
                er.Status = (int)apiException.StatusCode;
                er.Type = apiException.GetType().Name;
                er.Title = apiException.ReasonPhrase ?? er.Type;
                er.Detail = apiException.Content ?? er.Detail;
                break;
            default:
                er.Status = (int)HttpStatusCode.InternalServerError;
                er.Type = exception.GetType().Name;
                er.Title = "Internal Server Error";
                break;
        }

        httpContext.Response.StatusCode = er.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(er, cancellationToken);

        return true;
    }
}
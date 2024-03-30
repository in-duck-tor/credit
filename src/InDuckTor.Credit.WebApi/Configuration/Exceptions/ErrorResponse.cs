using System.Text.Json;

namespace InDuckTor.Credit.WebApi.Configuration.Exceptions;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";

    public void SetType(Exception exception) => Type = exception.GetType().Name;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
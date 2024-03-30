using System.Text.Json;

namespace InDuckTor.Credit.WebApi.Configuration.Exceptions;

public class ApiExceptionResponse
{
    public int Status { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
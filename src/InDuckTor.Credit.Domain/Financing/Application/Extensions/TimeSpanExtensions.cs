namespace InDuckTor.Credit.Domain.Financing.Application.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan MultipleOf(this TimeSpan changeable, TimeSpan multiple)
    {
        var round = Math.Round(changeable / multiple);
        if (round == 0) return multiple;
        return multiple * round;
    }
}
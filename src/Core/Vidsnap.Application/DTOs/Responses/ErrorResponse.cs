namespace Vidsnap.Application.DTOs.Responses
{
    public record ErrorResponse(string Message, ExceptionResponse? Exception);

    public class ExceptionResponse(Exception exception)
    {
        public string? Type { get; private set; } = exception.GetType().FullName;
        public string? Stack { get; private set; } = exception.StackTrace;
        public string? TargetSite { get; private set; } = exception.TargetSite?.ToString();
    }
}

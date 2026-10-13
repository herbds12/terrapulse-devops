namespace TerraPulse.Api.ViewModels.Common;

public class ValidationErrorResponseViewModel
{
    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
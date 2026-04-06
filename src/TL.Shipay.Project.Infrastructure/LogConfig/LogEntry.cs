namespace TL.Shipay.Project.Infrastructure.LogConfig
{
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? SourceContext { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public int? StatusCode { get; set; }
        public double? ElapsedMs { get; set; }
    }
}

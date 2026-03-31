namespace TL.Shipay.Project.IoC.Log
{
    public class LoggingShared
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string SourceContext { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}

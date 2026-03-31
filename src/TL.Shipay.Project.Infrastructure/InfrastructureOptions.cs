namespace TL.Shipay.Project.Infrastructure
{
    public class InfrastructureOptions
    {
        public ResilienciaConfig ResilienciaConfig { get; set; }
        public SerilogConfig SerilogConfig { get; set; }
    }

    public class ResilienciaConfig
    {
        public ServicoResilienciaConfig Services { get; set; }
        public int RetryCount { get; set; }
        public int RetryDelaySeconds { get; set; }
        public bool RetryUseJitter { get; set; }
        public double CircuitBreakerFailureRatio { get; set; }
        public int CircuitBreakerMinimumThroughput { get; set; }
        public double CircuitBreakerSamplingDuration { get; set; }
        public double CircuitBreakerBreakDuration { get; set; }
    }

    public class SerilogConfig
    {
        public string? Path { get; set; }
    }

    public class ServicoResilienciaConfig
    {
        public string ServicePrincipal { get; set; }
        public string ServiceFallback { get; set; }
    }
}

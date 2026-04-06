using Serilog;
using Serilog.Configuration;
using TL.Shipay.Project.Infrastructure.LogConfig;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class NamedPipeSinkExtensions
    {
        public static LoggerConfiguration NamedPipe(
            this LoggerSinkConfiguration sinkConfiguration,
            string pipeName = "serilog-log-pipe")
        {
            return sinkConfiguration.Sink(new NamedPipeSink(pipeName));
        }
    }
}

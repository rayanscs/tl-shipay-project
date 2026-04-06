using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace TL.Shipay.Project.Infrastructure.LogConfig
{
    public sealed class NamedPipeSink : ILogEventSink, IDisposable
    {
        private readonly ConcurrentQueue<LogEntry> _queue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _workerTask;
        private readonly string _pipeName;

        public NamedPipeSink(string pipeName = "serilog-log-pipe")
        {
            _pipeName = pipeName;
            _workerTask = Task.Run(ProcessQueueAsync);
        }

        /// <summary>
        /// Chamado pelo Serilog a cada evento de log.
        /// Apenas enfileira — nunca bloqueia.
        /// </summary>
        public void Emit(LogEvent logEvent)
        {
            var entry = new LogEntry
            {
                Timestamp = logEvent.Timestamp.UtcDateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                Exception = logEvent.Exception?.ToString(),
                SourceContext = GetProperty(logEvent, "SourceContext"),
                RequestPath = GetProperty(logEvent, "RequestPath"),
                RequestMethod = GetProperty(logEvent, "RequestMethod"),
                StatusCode = GetIntProperty(logEvent, "StatusCode"),
                ElapsedMs = GetDoubleProperty(logEvent, "Elapsed")
            };

            _queue.Enqueue(entry);
        }

        /// <summary>
        /// Worker assíncrono que consome a fila e envia pelo pipe.
        /// Reconecta automaticamente se o listener cair.
        /// </summary>
        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await using var pipeClient = new NamedPipeClientStream(
                        ".", _pipeName, PipeDirection.Out, PipeOptions.Asynchronous);

                    // Tenta conectar ao listener (timeout de 2s para não travar)
                    await pipeClient.ConnectAsync(2000, _cts.Token);

                    await using var writer = new StreamWriter(pipeClient, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };

                    while (pipeClient.IsConnected && !_cts.Token.IsCancellationRequested)
                    {
                        if (_queue.TryDequeue(out var entry))
                        {
                            var json = JsonSerializer.Serialize(entry);
                            await writer.WriteLineAsync(json);
                        }
                        else
                        {
                            // Sem logs na fila, espera um pouco
                            await Task.Delay(50, _cts.Token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // Listener não disponível, espera e tenta reconectar
                    await Task.Delay(1000, _cts.Token);
                }
            }
        }

        private static string? GetProperty(LogEvent logEvent, string name)
        {
            return logEvent.Properties.TryGetValue(name, out var value)
                ? value.ToString().Trim('"')
                : null;
        }

        private static int? GetIntProperty(LogEvent logEvent, string name)
        {
            if (logEvent.Properties.TryGetValue(name, out var value)
                && int.TryParse(value.ToString(), out var result))
                return result;
            return null;
        }

        private static double? GetDoubleProperty(LogEvent logEvent, string name)
        {
            if (logEvent.Properties.TryGetValue(name, out var value)
                && double.TryParse(value.ToString(), out var result))
                return result;
            return null;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}

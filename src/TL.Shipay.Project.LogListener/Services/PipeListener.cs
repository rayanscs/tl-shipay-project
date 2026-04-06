using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using TL.Shipay.Project.Infrastructure.LogConfig;

namespace TL.Shipay.Project.LogListener.Services
{
    public sealed class PipeListener
    {
        private readonly LogFileWriter _writer;
        private readonly string _pipeName;
        private long _totalLogsReceived;

        public PipeListener(LogFileWriter writer, string pipeName)
        {
            _writer = writer;
            _pipeName = pipeName;
        }

        public async Task ListenAsync(CancellationToken ct)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LogListener] Escutando no pipe: {_pipeName}");
            Console.WriteLine("[LogListener] Aguardando conexão da API...");
            Console.ResetColor();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await using var pipeServer = new NamedPipeServerStream(
                        _pipeName,
                        PipeDirection.In,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    await pipeServer.WaitForConnectionAsync(ct);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[LogListener] API conectada! Recebendo logs...");
                    Console.ResetColor();

                    await ProcessConnectionAsync(pipeServer, ct);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[LogListener] API desconectou. Aguardando reconexão...");
                    Console.ResetColor();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[LogListener] Erro: {ex.Message}");
                    Console.ResetColor();
                    await Task.Delay(1000, ct);
                }
            }
        }

        private async Task ProcessConnectionAsync(NamedPipeServerStream pipe, CancellationToken ct)
        {
            using var reader = new StreamReader(pipe, Encoding.UTF8);

            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(ct);

                if (line is null) break; // Conexão fechada

                try
                {
                    var entry = JsonSerializer.Deserialize<LogEntry>(line);
                    if (entry is not null)
                    {
                        await _writer.WriteAsync(entry, ct);
                        Interlocked.Increment(ref _totalLogsReceived);

                        // Feedback visual no console
                        PrintLogToConsole(entry);
                    }
                }
                catch (JsonException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[LogListener] JSON inválido: {line[..Math.Min(100, line.Length)]}");
                    Console.ResetColor();
                }
            }
        }

        private void PrintLogToConsole(LogEntry entry)
        {
            var color = entry.Level switch
            {
                "Error" or "Fatal" => ConsoleColor.Red,
                "Warning" => ConsoleColor.DarkYellow,
                "Information" => ConsoleColor.White,
                _ => ConsoleColor.Gray
            };

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[#{Interlocked.Read(ref _totalLogsReceived)}] ");
            Console.ForegroundColor = color;
            Console.WriteLine($"[{entry.Level}] {entry.Message}");
            Console.ResetColor();
        }
    }
}

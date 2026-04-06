using System.Globalization;
using System.Text;
using TL.Shipay.Project.Infrastructure.LogConfig;

namespace TL.Shipay.Project.LogListener.Services
{
    public sealed class LogFileWriter : IDisposable
    {
        private readonly string _baseDirectory;
        private StreamWriter? _currentWriter;
        private string _currentDate = string.Empty;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public LogFileWriter(string baseDirectory = "logs")
        {
            _baseDirectory = Path.GetFullPath(baseDirectory);
            Directory.CreateDirectory(_baseDirectory);
        }

        /// <summary>
        /// Escreve um LogEntry no arquivo. Thread-safe com SemaphoreSlim.
        /// Rotaciona o arquivo quando o dia muda.
        /// </summary>
        public async Task WriteAsync(LogEntry entry, CancellationToken ct = default)
        {
            await _semaphore.WaitAsync(ct);
            try
            {
                var today = entry.Timestamp.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (today != _currentDate)
                {
                    await RotateFileAsync(today);
                }

                var line = FormatLogLine(entry);
                await _currentWriter!.WriteLineAsync(line.AsMemory(), ct);
                await _currentWriter.FlushAsync(ct);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task RotateFileAsync(string date)
        {
            if (_currentWriter is not null)
            {
                await _currentWriter.DisposeAsync();
            }

            _currentDate = date;
            var filePath = Path.Combine(_baseDirectory, $"app-{date}.log");
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            _currentWriter = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = false };

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[LogListener] Escrevendo em: {filePath}");
            Console.ResetColor();
        }

        private static string FormatLogLine(LogEntry entry)
        {
            var sb = new StringBuilder();

            sb.Append('[');
            sb.Append(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            sb.Append("] [");
            sb.Append(entry.Level.PadRight(11));
            sb.Append(']');

            if (!string.IsNullOrEmpty(entry.SourceContext))
            {
                sb.Append(" [");
                sb.Append(entry.SourceContext);
                sb.Append(']');
            }

            if (!string.IsNullOrEmpty(entry.RequestMethod))
            {
                sb.Append($" {entry.RequestMethod} {entry.RequestPath}");

                if (entry.StatusCode.HasValue)
                    sb.Append($" -> {entry.StatusCode}");

                if (entry.ElapsedMs.HasValue)
                    sb.Append($" ({entry.ElapsedMs:0.00}ms)");
            }

            sb.Append(" | ");
            sb.Append(entry.Message);

            if (!string.IsNullOrEmpty(entry.Exception))
            {
                sb.AppendLine();
                sb.Append("  EXCEPTION: ");
                sb.Append(entry.Exception);
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            _currentWriter?.Dispose();
            _semaphore.Dispose();
        }
    }
}

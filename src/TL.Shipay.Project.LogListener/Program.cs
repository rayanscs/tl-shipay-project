using System.Text;
using TL.Shipay.Project.Infrastructure.LogConfig;
using TL.Shipay.Project.LogListener.Services;

Console.WriteLine("Shipay TL Test - Log Listener");

Console.OutputEncoding = Encoding.UTF8;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════╗");
Console.WriteLine("║     Serilog Log Listener — Console App      ║");
Console.WriteLine("║     Pressione Ctrl+C para encerrar          ║");
Console.WriteLine("╚══════════════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

// Cancellation token para shutdown gracioso
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\n[LogListener] Encerrando...");
};

// Pasta onde os logs serão salvos
var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
using var writer = new LogFileWriter(logDirectory);
var listener = new PipeListener(writer, PipeConstants.PipeName);

try
{
    await listener.ListenAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Shutdown normal
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("[LogListener] Encerrado com sucesso.");
Console.ResetColor();
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace JsonLinq.Benchmarks;

/// <summary>
/// Custom BenchmarkDotNet configuration for JsonLinq benchmarks.
/// </summary>
public sealed class JsonLinqBenchmarkConfig : ManualConfig
{
    public JsonLinqBenchmarkConfig()
    {
        // ManualConfig drops all BenchmarkDotNet defaults; loggers must be added explicitly
        AddLogger(ConsoleLogger.Default);

        // Restore default column providers (ManualConfig strips these)
        AddColumnProvider(DefaultColumnProviders.Instance);

        // Diagnosers for memory allocation tracking
        AddDiagnoser(MemoryDiagnoser.Default);

        // Exporters for report generation
        AddExporter(JsonExporter.Full);
        AddExporter(JsonExporter.BriefCompressed);
        AddExporter(HtmlExporter.Default);
        AddExporter(MarkdownExporter.GitHub);

        // Short run job for development/quick feedback (3 warmups, 3 iterations)
        AddJob(Job.ShortRun.AsDefault());
    }
}

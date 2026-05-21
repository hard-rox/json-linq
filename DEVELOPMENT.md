# Development

## Prerequisites

- .NET 10 SDK

## Build

```bash
dotnet build JsonLinq.slnx
```

## Test

```bash
dotnet test tests/JsonLinq.Tests/JsonLinq.Tests.csproj
```

## Run Example App

```bash
dotnet run --project examples/JsonLinq.Examples/JsonLinq.Examples.csproj
```

## Benchmarks

Run performance benchmarks (see [benchmarks/README.md](benchmarks/README.md) for details):

```bash
dotnet run -p benchmarks/JsonLinq.Benchmarks/ -c Release
```

Results are generated to `BenchmarkDotNet.Artifacts/results/index.html`.

## Suggested Copilot Skills from awesome-copilot

- csharp-xunit
- dotnet-best-practices
- csharp-async (for future async support)
- dotnet-upgrade (for framework maintenance)
- aspnet-minimal-api-openapi (for future REST API)


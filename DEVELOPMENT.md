# Development

## Prerequisites

- .NET 10 SDK
- To run tests locally for all targets, install runtimes for `net6.0`, `net8.0`, and `net9.0` as well.

## Build

```bash
dotnet build JsonLinq.slnx
```

## Test

```bash
dotnet test tests/JsonLinq.Tests/JsonLinq.Tests.csproj
```

Run a specific target framework:

```bash
dotnet test tests/JsonLinq.Tests/JsonLinq.Tests.csproj -f net10.0
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

## Publishing

Releases are published to NuGet.org and GitHub Packages by pushing a `vX.Y.Z`
tag. Publishing uses NuGet Trusted Publishing (OIDC, keyless). See
[docs/publishing.md](docs/publishing.md) for the full release process and setup.

## Suggested Copilot Skills from awesome-copilot

- csharp-xunit
- dotnet-best-practices
- csharp-async (for future async support)
- dotnet-upgrade (for framework maintenance)
- aspnet-minimal-api-openapi (for future REST API)


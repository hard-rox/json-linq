# Plan: JsonLinq .NET Class Library Implementation

## TL;DR
Create a complete, production-ready .NET 10 class library package called **JsonLinq** that ports the JavaScript/PHP jsonq functionality. The project will follow open-source standards with fluent LINQ-like API, comprehensive documentation, xunit tests with NSubstitute mocking, example console app, and full GitHub CI/CD pipeline including CodeQL, dependabot, and multi-registry publishing (NuGet.org + GitHub Packages).

---

## Project Specifications

**Package Name**: JsonLinq  
**Solution Format**: .slnx (modern format)  
**Target Framework**: .NET 10  
**Test Framework**: xunit with NSubstitute (not FluentAssertions)  
**License**: MIT  
**Publish**: Both NuGet.org and GitHub Packages  
**Key Design**: Fluent LINQ-like API for querying JSON data  

---

## Directory Structure

```
JsonLinq/
├── .github/
│   ├── workflows/
│   │   ├── codeql.yml           # CodeQL security scanning
│   │   ├── dependabot.yml       # Dependency updates
│   │   └── publish.yml          # CI/CD: test, build, publish
│   └── ISSUE_TEMPLATE/
│       └── config.yml           # Issue templates
├── docs/
│   ├── getting-started.md       # Quick start guide
│   ├── api-reference.md         # Detailed API documentation
│   ├── examples.md              # Usage examples
│   ├── architecture.md          # Design patterns and internals
│   ├── contributing.md          # Contribution guidelines
│   └── CHANGELOG.md             # Version history
├── src/
│   └── JsonLinq/
│       ├── JsonLinq.csproj      # Class library
│       ├── Abstractions/
│       │   ├── IQueryable.cs    # Core interfaces
│       │   └── ICondition.cs
│       ├── Builders/
│       │   ├── QueryBuilder.cs  # Fluent query builder
│       │   └── ConditionBuilder.cs
│       ├── Core/
│       │   ├── JsonQuery.cs     # Main entry point
│       │   ├── QueryEngine.cs   # Query execution
│       │   └── Matcher.cs       # Condition matching
│       ├── Extensions/
│       │   └── LinqExtensions.cs # LINQ-style helpers
│       ├── Exceptions/
│       │   ├── JsonQueryException.cs
│       │   └── InvalidPathException.cs
│       ├── Utilities/
│       │   ├── JsonParser.cs    # JSON parsing
│       │   └── PathResolver.cs  # Path navigation
│       └── GlobalUsings.cs      # Global imports
├── examples/
│   └── JsonLinq.Examples/
│       ├── JsonLinq.Examples.csproj
│       ├── Program.cs           # Example console app
│       ├── Scenarios/           # Example scenarios
│       │   ├── BasicFiltering.cs
│       │   ├── Aggregations.cs
│       │   ├── Grouping.cs
│       │   └── Sorting.cs
│       └── data.json            # Sample JSON file
├── tests/
│   └── JsonLinq.Tests/
│       ├── JsonLinq.Tests.csproj
│       ├── Unit/
│       │   ├── QueryBuilderTests.cs
│       │   ├── JsonQueryTests.cs
│       │   ├── MatcherTests.cs
│       │   ├── JsonParserTests.cs
│       │   └── PathResolverTests.cs
│       ├── Integration/
│       │   └── EndToEndTests.cs
│       ├── Fixtures/
│       │   ├── SampleDataFixture.cs
│       │   └── JsonTestHelper.cs
│       └── Data/
│           └── sample-data.json
├── .copilot-instructions.md     # Copilot agent instructions
├── .gitignore                   # Git ignore patterns
├── JsonLinq.sln                 # Old format (legacy, won't create)
├── JsonLinq.slnx                # Modern solution format
├── Directory.Build.props        # Common project properties
├── Directory.Packages.props     # Central package management
├── DEVELOPMENT.md              # Developer setup guide
├── LICENSE                      # MIT License
├── README.md                    # Project overview
└── nuget.config                 # NuGet configuration

```

---

## Implementation Steps

### Phase 1: Project Setup & Structure (Steps 1-5)

1. **Create solution in slnx format**
   - Initialize `JsonLinq.slnx` in workspace root
   - Set TargetFramework to net10.0
   - Configure shared project properties via `Directory.Build.props`
   - Enable nullable reference types and latest C# features

2. **Create core class library** (`src/JsonLinq/JsonLinq.csproj`)
   - Reference: `System.Text.Json` (built-in)
   - Use nullable annotations enabled
   - Enable deterministic builds for reproducibility

3. **Create test project** (`tests/JsonLinq.Tests/JsonLinq.Tests.csproj`)
   - Package dependencies: xunit, xunit.runner.visualstudio, NSubstitute
   - Reference JsonLinq library
   - Use xunit test discovery

4. **Create example console app** (`examples/JsonLinq.Examples/`)
   - Reference JsonLinq library
   - Target same framework (net10.0)
   - Include sample JSON data file

5. **Configure central package management** (`Directory.Packages.props`)
   - Lock versions: xunit, NSubstitute, System.Text.Json
   - Enable transitive dependency management

### Phase 2: Core Implementation (Steps 6-12) — *depends on Phase 1*

6. **Implement abstractions** (`Abstractions/`)
   - `IQueryable` - Main query interface with chainable methods
   - `ICondition` - Condition representation for where/orWhere
   - `IQueryResult` - Result wrapper for get(), count(), etc.

7. **Implement core engine** (`Core/`)
   - `JsonQuery` - Main entry point (constructor takes JSON source)
   - `QueryEngine` - Execution logic for all operations
   - `Matcher` - Evaluates conditions against data items

8. **Implement utilities** (`Utilities/`)
   - `JsonParser` - Load from string, file, or object
   - `PathResolver` - Navigate nested paths (supports dot notation)
   - Handle edge cases: null values, missing paths, type coercion

9. **Implement builders** (`Builders/`)
   - `QueryBuilder` - Fluent API for method chaining
   - Methods: `from()`, `where()`, `orWhere()`, `sort()`, `groupBy()`, `chunk()`, etc.
   - Return `this` for chaining; implement IDisposable for resource cleanup

10. **Implement extensions** (`Extensions/`)
    - Shortcut methods mimicking LINQ (e.g., `.Where()` vs `.where()`)
    - Static factory methods: `JsonLinq.From()`, `JsonLinq.FromFile()`

11. **Define exceptions** (`Exceptions/`)
    - `JsonQueryException` - Base exception
    - `InvalidPathException` - Path resolution errors
    - `InvalidOperationException` for aggregation on non-numeric types

12. **Add XML documentation**
    - All public types, methods, properties (enable doc comment generation)
    - Include remarks on performance implications
    - Add code examples in documentation

### Phase 3: Comprehensive Testing (Steps 13-17) — *depends on Phase 2*

13. **Unit tests: Core functionality** (`tests/Unit/JsonQueryTests.cs`)
    - Test `from()`, `find()`, `at()` path navigation
    - Test with nested objects, arrays, mixed structures
    - Test edge cases: empty arrays, null values, non-existent paths
    - Use xunit `[Theory]` with `[InlineData]` for multiple scenarios

14. **Unit tests: Filtering** (`tests/Unit/QueryBuilderTests.cs`)
    - Test `where()` with operators: `==`, `!=`, `>`, `<`, `>=`, `<=`, `in`, `contains`
    - Test `orWhere()` combining multiple conditions
    - Use NSubstitute to mock `Matcher` if needed
    - Test complex condition nesting

15. **Unit tests: Aggregations & Transformations** (within `QueryBuilderTests.cs`)
    - Test `sum()`, `count()`, `max()`, `min()`, `avg()` on valid data
    - Test error handling on non-numeric aggregations
    - Test `sort()`, `sortBy()`, `groupBy()`, `chunk()`
    - Test `first()`, `last()`, `nth()` boundary conditions

16. **Unit tests: Utilities** (`tests/Unit/JsonParserTests.cs`, `PathResolverTests.cs`)
    - `JsonParser`: Load from string/file/object, validate JSON
    - `PathResolver`: Navigate "users.0.name", handle missing keys gracefully
    - Use `[MemberData]` for path scenarios

17. **Integration tests** (`tests/Integration/EndToEndTests.cs`)
    - Full query chains: from().where().sort().groupBy().get()
    - Load sample JSON, run complex queries, verify results
    - Use `IClassFixture<SampleDataFixture>` for shared test data
    - Compare against expected results

### Phase 4: Examples & Documentation (Steps 18-21) — *depends on Phase 2*

18. **Create example console app** (`examples/JsonLinq.Examples/`)
    - Scenario 1: `BasicFiltering.cs` - Simple where/get
    - Scenario 2: `Aggregations.cs` - sum/count/avg on dataset
    - Scenario 3: `Grouping.cs` - groupBy with nested results
    - Scenario 4: `Sorting.cs` - sort/sortBy with multiple columns
    - Each scenario loads sample data and demonstrates API

19. **Create core documentation**
    - `README.md` - Project overview, quick start, feature list
    - `getting-started.md` - Installation, first query example
    - `api-reference.md` - Method signatures, parameters, return types
    - `examples.md` - Copy examples from console app with explanations
    - `DEVELOPMENT.md` - Local setup, running tests, build commands

20. **Create architecture documentation**
    - `architecture.md` - Design patterns: Builder, Engine, Matcher, Factory
    - Class diagram (describe relationships in text)
    - Explain LINQ-style design choices vs php/js jsonq
    - Performance considerations

21. **Create meta-documentation**
    - `contributing.md` - PR process, code style, testing requirements
    - `CHANGELOG.md` - Version history, template for future entries
    - `LICENSE` - MIT license text

### Phase 5: GitHub Configuration (Steps 22-25) — *parallel with Phase 4*

22. **Create CodeQL workflow** (`.github/workflows/codeql.yml`)
    - Trigger: on push (main branch), on pull_request, weekly schedule
    - Run CodeQL analysis for C#
    - Upload SARIF results to GitHub Security tab
    - Reference: Standard GitHub template

23. **Create dependabot configuration** (`.github/dependabot.yml`)
    - Enable version updates for nuget (weekly)
    - Enable security updates for nuget (daily)
    - Group dependencies: "dev-dependencies", "production"
    - Auto-merge patch updates (optional)

24. **Create CI/CD publish workflow** (`.github/workflows/publish.yml`)
    - Trigger: on push (main branch tags v*), on workflow_dispatch
    - Steps:
      - Checkout code
      - Setup .NET 10
      - Restore, build, test
      - Pack NuGet package (`dotnet pack`)
      - Publish to NuGet.org (requires NUGET_API_KEY secret)
      - Publish to GitHub Packages (uses GITHUB_TOKEN)

25. **Create GitHub repository metadata**
    - `.gitignore` - Ignore bin/, obj/, .vs/, .idea/, *.user
    - `.github/ISSUE_TEMPLATE/config.yml` - Issue templates for bug/feature/discussion
    - Add branch protection rules (document in DEVELOPMENT.md)

### Phase 6: Copilot Customization (Steps 26-28) — *can be parallel*

26. **Create Copilot instructions** (`.copilot-instructions.md`)
    - YAML frontmatter: domain=JsonLinq, enabledSkills=[]
    - Instructions for maintaining LINQ-style fluent API
    - Enforce XML documentation on all public members
    - Link to architecture.md and contributing.md
    - Style guide: method naming (lowercase verbs), immutability preferences

27. **Create or reference skills**
    - Use existing: `csharp-xunit`, `dotnet-best-practices`
    - Create custom skill: `.vscode/skills/jsonlinq-api-design/SKILL.md`
      - Enforce Builder pattern with method chaining
      - Ensure consistent parameter ordering
      - Validate LINQ-style aliases and overloads

28. **Document awesome-copilot skills to add** (in DEVELOPMENT.md)
    - Suggest: `aspnet-minimal-api-openapi` (future REST API support)
    - Suggest: `csharp-async` (if adding async variants)
    - Suggest: `dotnet-upgrade` (for framework maintenance)

---

## Relevant Files to Create/Modify

**Project Files**:
- `JsonLinq.slnx` — Solution file (modern format)
- `Directory.Build.props` — Shared .NET configuration
- `Directory.Packages.props` — Central NuGet version management
- `nuget.config` — NuGet feed configuration (GitHub Packages, NuGet.org)

**Core Library** (`src/JsonLinq/`):
- `Abstractions/IQueryable.cs` — Define fluent API contract
- `Core/JsonQuery.cs` — Main entry point, composition root
- `Core/QueryEngine.cs` — Query execution logic (where, sort, group, chunk, aggregate)
- `Core/Matcher.cs` — Condition evaluation (where operators)
- `Builders/QueryBuilder.cs` — Fluent builder pattern implementation
- `Utilities/JsonParser.cs` — JSON source handling
- `Utilities/PathResolver.cs` — Dot-notation path navigation
- `Exceptions/*.cs` — Custom exception types

**Tests** (`tests/JsonLinq.Tests/`):
- `Unit/JsonQueryTests.cs` — Path navigation, from/find/at
- `Unit/QueryBuilderTests.cs` — Filtering, sorting, grouping, aggregation
- `Unit/MatcherTests.cs` — Condition matching operators
- `Unit/JsonParserTests.cs` — JSON loading edge cases
- `Unit/PathResolverTests.cs` — Path resolution scenarios
- `Integration/EndToEndTests.cs` — Complex query chains
- `Fixtures/SampleDataFixture.cs` — Shared test data using xunit fixtures

**Examples** (`examples/JsonLinq.Examples/`):
- `Program.cs` — Main entry point, scenario runner
- `Scenarios/BasicFiltering.cs` — Simple query example
- `Scenarios/Aggregations.cs` — sum, count, avg, min, max
- `Scenarios/Grouping.cs` — groupBy demonstration
- `Scenarios/Sorting.cs` — sort/sortBy examples
- `data.json` — Sample dataset (users, products, etc.)

**Documentation**:
- `README.md` — Package overview
- `docs/getting-started.md` — Quick start
- `docs/api-reference.md` — Full API documentation
- `docs/examples.md` — Usage examples from console app
- `docs/architecture.md` — Design patterns and internals
- `docs/contributing.md` — Contribution process
- `DEVELOPMENT.md` — Developer setup

**GitHub Workflows**:
- `.github/workflows/codeql.yml` — Security scanning
- `.github/workflows/dependabot.yml` — Dependency updates (config)
- `.github/workflows/publish.yml` — Build, test, pack, publish
- `.github/ISSUE_TEMPLATE/config.yml` — Issue templates

**Copilot & Developer Experience**:
- `.copilot-instructions.md` — Copilot behavior for JsonLinq
- `.vscode/settings.json` — Workspace settings (optional)
- `.gitignore` — Standard .NET exclusions

---

## Verification Steps

### Build & Compilation
1. `dotnet build JsonLinq.slnx` — Solution builds without errors
2. `dotnet pack src/JsonLinq/JsonLinq.csproj` — Generates .nupkg
3. Verify package metadata in .nupkg (version, description, icons, project URL)

### Tests
4. `dotnet test tests/JsonLinq.Tests/JsonLinq.Tests.csproj` — All tests pass
5. Verify test coverage includes:
   - Path navigation (from, find, at)
   - Filtering (where, orWhere, operators)
   - Aggregations (sum, count, max, min, avg)
   - Transformations (sort, sortBy, groupBy, chunk)
   - Access methods (first, last, nth)
   - Edge cases and error handling

### Examples
6. `dotnet run --project examples/JsonLinq.Examples/` — Console app runs without errors
7. Verify each scenario produces expected output

### Documentation
8. XML documentation generates without warnings: `dotnet build /p:DocumentationFile=bin/Release/net10.0/JsonLinq.xml`
9. All public types/methods have complete XML doc comments
10. README renders correctly on GitHub (headers, code blocks, links)
11. Links in docs (e.g., `docs/api-reference.md`) work correctly

### GitHub Integration
12. CodeQL workflow runs on PR: Check GitHub Security tab for results
13. Dependabot creates initial PR with discovered vulnerabilities
14. Publish workflow triggers on version tag: Verify package in NuGet.org and GitHub Packages
15. Branch protection enforces: status checks pass, code review required

---

## Decisions & Architecture

### 1. **Fluent LINQ-like API Design**
- **Decision**: Implement chainable methods returning `this` or new instances
- **Rationale**: Matches .NET developer expectations, differs from js-jsonq verbosity
- **Example**: `JsonLinq.From(json).Where("age", ">", 25).Sort("name").Get()`

### 2. **Immutability vs Mutability**
- **Decision**: Each operation returns new instances (immutable); no `reset()` method
- **Rationale**: Safer for concurrent use, prevents accidental state changes
- **Note**: `.copy()` method becomes redundant in .NET; not implemented

### 3. **Operator String vs Enum**
- **Decision**: Use string operators ("==", ">", "in") for simplicity
- **Rationale**: Matches php-jsonq and js-jsonq; easier for JSON-driven queries
- **Alternative considered**: Strongly-typed enum operators (rejected for compatibility)

### 4. **Generic vs Object-based Results**
- **Decision**: Core returns `IEnumerable<dynamic>` or `List<object>`; allow projection
- **Rationale**: JSON is untyped; generics added via LINQ extensions for type safety
- **Example**: `.Get<User>()` for type-safe projections (future enhancement)

### 5. **Exception Strategy**
- **Decision**: Throw specific exceptions; no silent failures
- **Rationale**: Better debugging experience than null/default returns
- **Examples**: `JsonQueryException`, `InvalidPathException`

### 6. **Testing: NSubstitute vs Moq**
- **Decision**: Use NSubstitute for mocking (user requirement)
- **Rationale**: Cleaner fluent syntax, good documentation
- **Note**: Standard xunit Assert.* (no FluentAssertions per requirement)

### 7. **Package Publishing**
- **Decision**: Publish to both NuGet.org and GitHub Packages
- **Rationale**: Maximum discoverability; users can choose package source
- **CI/CD**: Single workflow, conditional secrets for both registries

### 8. **Solution Format**
- **Decision**: Use modern `.slnx` format
- **Rationale**: Lighter, better for monorepo scenarios, forward-compatible
- **Note**: Still includes `.sln` in documentation as fallback

### Scope Inclusions
✅ Core query engine (from, where, sort, groupBy, chunk, aggregations)  
✅ LINQ-style fluent API  
✅ Comprehensive tests with xunit + NSubstitute  
✅ Example console app  
✅ Full documentation (getting-started, API, architecture, contributing)  
✅ GitHub CI/CD (CodeQL, dependabot, publish)  
✅ Copilot instructions + custom skill  
✅ MIT license, open-source standards  

### Scope Exclusions
❌ Async variants (future enhancement)  
❌ YAML, CSV, XML support (jsonq ecosystem feature; out of scope)  
❌ REST API wrapper (considered for future)  
❌ Benchmarking suite (can add later)  
❌ Performance optimizations beyond reasonable (premature optimization)  

---

## Further Considerations

1. **API Naming Alignment**
   - Question: Keep lowercase method names (from js-jsonq compatibility) or use PascalCase (C# convention)?
   - **Recommendation**: Use PascalCase (`From()`, `Where()`) with lowercase aliases for compatibility. Prioritize C# conventions; power users can use `using static JsonLinq.QueryBuilder` for concise syntax.

2. **Async Support Future**
   - Question: Implement async variants now or defer?
   - **Recommendation**: Defer to v2.0. Current implementation is I/O lightweight (file reading is minimal). Add `FromAsync()`, `GetAsync()` after core v1.0 release if demand exists.

3. **Performance Testing**
   - Question: Include benchmark suite (BenchmarkDotNet) in initial release?
   - **Recommendation**: Defer. Focus on correctness; add benchmarks in v1.1 if performance becomes concern.

4. **Type Safety Enhancements**
   - Question: Add strongly-typed query builder (e.g., `Query<T>`) in v1.0 or defer?
   - **Recommendation**: Defer to v2.0. Current dynamic approach mirrors jsonq philosophy; typed variants can come later as extension package.

5. **Community & Governance**
   - Question: Include CODE_OF_CONDUCT.md, GOVERNANCE.md in initial release?
   - **Recommendation**: Include basic CODE_OF_CONDUCT.md (link to standard); full governance can evolve post-launch.

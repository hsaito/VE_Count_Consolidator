## Project context
- VE Count Consolidator aggregates Volunteer Examiner session counts; split into library VECountConsolidator/ and CLI VECountConsolidatorCli/.
- Library surface: Consolidator.Process() orchestrates VEC-specific ICountGetter implementations; current VEC enum only includes ARRL VECountConsolidator/Consolidator.cs.
- Data model: Person carries Call, Name, Count, State (code/name), Vec; State list populated per getter.

## ARRL implementation
- Web scrape flow: ARRL.Extract() loads state options from http://www.arrl.org/ve-session-counts, sets _baseUrl templates, fetches per-state tables via _webProvider, extracts <table> markup with regex, parses rows via XElement into Person entries VECountConsolidator/ARRL.cs.
- _webProvider injectable via ctor for tests; default uses Utils.GetWeb(url).Result HTTP request VECountConsolidator/Utils.cs. Inject a mock delegate to keep tests fast and offline.
- Tests assume 63 state options; if the ARRL page changes, adjust fixtures/expectations in VECountConsolidator.Tests/ArrlTests.cs and VECountConsolidator.Tests/ConsolidatorTests.cs.
- Unsupported VECs throw VECountConsolidatorException; extend by adding an enum value, implementing ICountGetter, and wiring into Process().

## CLI usage
- Entry point in VECountConsolidatorCli/Program.cs via CommandLine parser; only mode create is honored.
- RunProcess() calls Consolidator.Process(ARRL) and writes output.csv using CsvHelper mapping in VECountConsolidatorCli/CsvProcessor.cs.
- Typical run: dotnet run --project VECountConsolidatorCli --mode create; output file lands in the working directory.

## Build/test
- Build: dotnet build VE_Count_Consolidator.sln.
- Tests: dotnet test (xUnit). Web calls are mocked; avoid real HTTP in tests.

## Conventions & notes
- Keep responses in English.
- Network access uses WebRequest; errors bubble as exceptions. When adding new getters, follow the injectable provider pattern to remain testable.
- CSV schema columns: Callsign, Name, State (name), Count, VEC.
- Security: when introducing new C# code, run snyk_code_scan on touched files and fix reported issues before completion.
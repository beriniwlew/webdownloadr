# Tests

This folder contains all test projects used to verify the application.

- **WebDownloadr.UnitTests** – unit tests for the Core, UseCases and Web layers.
- **WebDownloadr.IntegrationTests** – verifies Infrastructure with a database.
- **WebDownloadr.FunctionalTests** – exercises the API using `WebApplicationFactory`.
- **WebDownloadr.ArchTests** – enforces architecture rules.
- **WebDownloadr.AspireTests** – runs Aspire host scenarios.

Run all tests:

```bash
dotnet test WebDownloadr.sln
```

Run a single project:

```bash
dotnet test tests/WebDownloadr.UnitTests
```

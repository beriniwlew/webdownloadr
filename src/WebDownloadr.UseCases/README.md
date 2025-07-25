# WebDownloadr.UseCases

This project contains the application's use cases. Handlers here coordinate the domain model exposed by `WebDownloadr.Core` without
depending on infrastructure concerns. Each use case is expressed as a command or query so it can be invoked from tests or from the API
layer.

## Web Pages

Operations against the `WebPage` aggregate reside under [`WebPages`](WebPages/README.md). This includes creating, listing and updating pages
as well as running downloads. The download-specific handlers are further documented in [`WebPages/Download`](WebPages/Download/README.md).

## Contributors

The `Contributors` folder demonstrates simple CRUD handlers for a secondary entity. These use cases are structured the same way as the ones
for web pages.

## Key Classes

Handlers are implemented as MediatR requests and invoke
`IDownloadWebPageService` when coordinating downloads.

## Running Tests

Run the use case tests with:

```bash
dotnet test tests/WebDownloadr.UnitTests
```

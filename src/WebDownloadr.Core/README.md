# WebDownloadr.Core

This project contains the domain model for WebDownloadr. It defines the entities, value objects and domain events that describe how web
pages are downloaded.

The main aggregate is documented in [`WebPageAggregate`](WebPageAggregate/README.md).

## Key Services

`DownloadWebPageService` implements `IDownloadWebPageService` and manages the
state of a `WebPage` while it downloads content.

## Running Tests

Execute the core unit tests with:

```bash
dotnet test tests/WebDownloadr.UnitTests
```

# WebDownloadr.Core

This project contains the domain model for WebDownloadr. It defines the entities, value objects and domain events that describe how web
pages are downloaded.

The main aggregate is documented in [`WebPageAggregate`](WebPageAggregate/README.md).

## Key Services

`IDownloadWebPageService` defines operations for downloading web pages and managing their lifecycle. Its implementation resides in this
project under `Services/DownloadWebPageService`.

## Running Tests

Execute the core unit tests with:

```bash
dotnet test tests/WebDownloadr.UnitTests
```

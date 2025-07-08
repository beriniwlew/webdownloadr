# WebPage Aggregate

The WebPage aggregate represents a web page that can be downloaded by the application. It is the core domain model used by the `DownloadWebPageService` and related use cases.

## Aggregate Components

- **WebPage** - The aggregate root containing the target `WebPageUrl`, a strongly typed `WebPageId`, and a `DownloadStatus` indicating the progress of the download.
- **WebPageId** - A value object generated with [Vogen](https://github.com/SteveDower/vogen) providing a type-safe identifier for `WebPage` entities.
- **WebPageUrl** - Value object that validates incoming URLs. Only valid HTTP or HTTPS URLs can be created.
- **DownloadStatus** - An enumeration derived from Ardalis SmartEnum describing the download lifecycle: queued, in progress, completed, cancelled or error.
- **WebPageDownloadedEvent** - Domain event raised when a download succeeds. The event contains the `WebPageId` and the downloaded HTML content.

## Typical Workflow

1. A `WebPage` is created using a valid `WebPageUrl`.
2. The `DownloadWebPageService` updates the entity's `DownloadStatus` as the download progresses.
3. Once the download completes successfully, `WebPageDownloadedEvent` is published so other parts of the system can react.

This aggregate encapsulates all state and behavior related to downloading a single web page and should be the only entry point for modifying that state.

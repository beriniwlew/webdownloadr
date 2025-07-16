# 0003: Web Pages Functionality Implementation

Status: Accepted  
Date: 2024-01-15  
Supersedes: N/A  
Superseded-by: N/A

## Context

The project requires functionality to download and manage web pages. This involves creating a system that can fetch web content, store it,
and provide management capabilities. The implementation needs to be scalable, maintainable, and follow the established Clean Architecture
patterns.

## Decision

Implement web page functionality using a domain-driven approach. A `WebPage` aggregate lives under `src/WebDownloadr.Core/WebPageAggregate/`
and encapsulates the page URL, download status and the `WebPageDownloadedEvent` domain event. The `DownloadWebPageService`
(`src/WebDownloadr.Core/Services/DownloadWebPageService.cs`) orchestrates downloads using an `IWebPageDownloader` implementation and
persists state changes via a repository. Download requests are handled by command handlers located in `src/WebDownloadr.UseCases.WebPages`,
which queue work through the service (`DownloadWebPageHandler`, `DownloadWebPagesHandler`, `CancelDownloadHandler`, `RetryDownloadHandler`).
The infrastructure project provides `SimpleWebPageDownloader` (`src/WebDownloadr.Infrastructure/Web/SimpleWebPageDownloader.cs`) that
fetches pages over HTTP and saves them locally. Domain events are published via `IMediator` so additional behaviors can react to a completed
download without tight coupling.

## Consequences

**Positive outcomes:**

- **Clear separation of concerns** through aggregates and use-case handlers
- **Queued downloads** via `DownloadWebPageService` keep HTTP requests fast and allow cancellation or retry
- **Domain events** let other features react to completed downloads asynchronously
- **CQRS-style handlers** provide optimal performance for reads and writes

**Negative outcomes:**

- Increased complexity compared to a simple CRUD approach
- Additional coordination code is required to manage concurrent downloads
- Learning curve for team members unfamiliar with event-driven patterns

**Follow-up tasks:**

Initial web page features, validation and event handling have been implemented.
Future enhancements may include allowing custom HTTP headers (e.g., a User-Agent) when downloading pages.

## Alternatives Considered

- **Simple CRUD approach** – Would be faster to implement but less scalable
- **Monolithic service approach** – Would violate Clean Architecture principles
- **Event sourcing** – Considered but rejected due to complexity for current requirements

## References

- [Domain-Driven Design Fundamentals](https://www.pluralsight.com/courses/fundamentals-domain-driven-design)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Clean Architecture Principles](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)

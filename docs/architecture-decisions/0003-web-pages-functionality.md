# 0003 – Web Pages Functionality

**Status**: Proposed

## Context

The Web Pages feature coordinates CRUD and download workflows for the `WebPage` aggregate. As noted in the [UseCases README](../../src/WebDownloadr.UseCases/README.md) lines 7–10, operations for `WebPage` are organized under the `WebPages` folder and cover creating, listing, updating pages and running downloads. The [Web README](../../src/WebDownloadr.Web/README.md) lines 7–10 lists download endpoints exposed by the Web project, including starting, cancelling and retrying downloads as well as bulk requests.

## Decision

Expose CRUD and download endpoints for pages via the Web project using FastEndpoints. Pages can be created, retrieved, updated, deleted and downloaded through the API following the routes documented in the Web README.

## Consequences

- Requires a `WebPage` domain type with associated EF Core table to persist page data and download status.
- UseCase handlers must implement create, list, update, delete and download commands and queries.
- Download requests trigger background processing to fetch page content and update status.
- API endpoints reflect this functionality, enabling integrations and UI clients to manage pages and downloads.

## Alternatives Considered

- Implement downloads only through an external CLI – rejected to keep functionality in a single API surface.
- Limit the feature to downloads without CRUD – rejected because managing tracked pages is essential.

## References

- `src/WebDownloadr.UseCases/README.md` lines 7–10
- `src/WebDownloadr.Web/README.md` lines 7–10

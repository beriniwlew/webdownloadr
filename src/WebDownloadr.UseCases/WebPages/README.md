# WebPage Use Cases

Handlers under this folder operate on the `WebPage` aggregate. They cover basic CRUD operations in addition to download workflows.

- **Create** – adds a new web page to track.
- **Get** – retrieves a single page by identifier.
- **List** – returns pages with optional paging.
- **Update** – updates the download status.
- **Delete** – removes a page.
- **Download** – starts, cancels or retries downloads (see [Download](Download/README.md)).

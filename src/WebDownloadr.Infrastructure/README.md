# WebDownloadr.Infrastructure

This project provides implementations for external dependencies used by the application such as HTTP clients or data stores. These services
implement interfaces defined in the Core or UseCases projects and are wired up at runtime by the Web project.

The `Web` folder contains `SimpleWebPageDownloader`, a configurable HTTP downloader used by `DownloadWebPageService`. It streams responses
directly to disk, limits concurrent downloads and names each file using the `WebPageId`.

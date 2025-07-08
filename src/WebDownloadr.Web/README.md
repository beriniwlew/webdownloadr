# WebDownloadr.Web

The Web project exposes API endpoints using FastEndpoints.
It wires up the application and infrastructure layers.

### Download Endpoints

- `POST /WebPages/{WebPageId}/download` — start downloading a page.
- `POST /WebPages/{WebPageId}/download/cancel` — cancel a running download.
- `POST /WebPages/{WebPageId}/download/retry` — retry a failed download.
- `POST /WebPages/download` — download multiple pages in one request.

Run the project and visit `/swagger` for full API documentation.

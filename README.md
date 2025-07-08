# WebDownloadr

WebDownloadr demonstrates how to apply Clean Architecture to a simple web page downloader built with .NET 9. The solution is organized into separate projects that isolate the domain model, application services, infrastructure and API.

## Projects

- **WebDownloadr.Core** – domain model containing the [`WebPage` aggregate](src/WebDownloadr.Core/WebPageAggregate/README.md).
- **WebDownloadr.UseCases** – application layer describing operations such as requesting downloads.
- **WebDownloadr.Infrastructure** – implementations of external dependencies like HTTP clients.
- **WebDownloadr.Web** – minimal API exposing endpoints.
- **Tests** – unit, integration and functional test projects.

## Getting Started

Restore packages and run the Web project:

```bash
dotnet run --project src/WebDownloadr.Web/WebDownloadr.Web.csproj
```

Browse to `/swagger` for API documentation while the app is running.

## Documentation

Additional documentation lives in the `docs` folder and within each project. Start with [`src/WebDownloadr.Core/README.md`](src/WebDownloadr.Core/README.md) to learn about the domain model.

## License

This project is licensed under the [MIT](LICENSE) license.


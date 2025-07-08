# WebDownloadr
![CI](https://github.com/beriniwlew/webdownloadr/actions/workflows/ci.yml/badge.svg?branch=main)

WebDownloadr demonstrates how to apply Clean Architecture to a simple web page downloader built with .NET 9. The solution is organized into separate projects that isolate the domain model, application services, infrastructure and API.

## Projects

- **WebDownloadr.Core** – domain model containing the [`WebPage` aggregate](src/WebDownloadr.Core/WebPageAggregate/README.md).
- **WebDownloadr.UseCases** – application layer describing operations such as requesting downloads (see [README](src/WebDownloadr.UseCases/README.md)).
- **WebDownloadr.Infrastructure** – implementations of external dependencies like HTTP clients.
- **WebDownloadr.Web** – minimal API exposing endpoints (see [README](src/WebDownloadr.Web/README.md)).
- **Tests** – unit, integration and functional test projects.

## Getting Started

Restore packages and run the Web project:

```bash
dotnet run --project src/WebDownloadr.Web/WebDownloadr.Web.csproj
```

Browse to `/swagger` for API documentation while the app is running.

## Documentation

Additional documentation lives in the `docs` folder and within each project. Start with [`src/WebDownloadr.Core/README.md`](src/WebDownloadr.Core/README.md) to learn about the domain model.

## Formatting

Formatting is enforced in CI. Configure Git to honor `.editorconfig` line endings:

```bash
git config --global core.autocrlf true
```

For the initial bootstrap, maintainers should normalize all files and format the solution:

```bash
git add --renormalize .
git commit -m "style: normalize line endings to match .editorconfig"
./scripts/bootstrap-format.sh --commit
```

After this commit, `./scripts/format.sh` (or `dotnet format`) runs in CI and must report no changes.

## License

This project is licensed under the [MIT](LICENSE) license.


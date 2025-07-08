using Ardalis.Result;
using NSubstitute;
using Shouldly;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.UseCases.WebPages.Download.CancelDownload;
using WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;
using WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;
using WebDownloadr.UseCases.WebPages.Download.RetryDownload;

namespace WebDownloadr.UnitTests.UseCases.WebPages.Download;

public class DownloadHandlers_Delegates
{
    private readonly IDownloadWebPageService _service = Substitute.For<IDownloadWebPageService>();

    [Fact]
    public async Task CancelDownloadHandler_DelegatesToService()
    {
        var handler = new CancelDownloadHandler(_service);
        var id = Guid.NewGuid();
        _service.CancelDownloadAsync(id, Arg.Any<CancellationToken>()).Returns(Result.Success(id));

        var result = await handler.Handle(new CancelDownloadCommand(id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _service.Received().CancelDownloadAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DownloadWebPageHandler_DelegatesToService()
    {
        var handler = new DownloadWebPageHandler(_service);
        var id = Guid.NewGuid();
        _service.DownloadWebPageAsync(id, Arg.Any<CancellationToken>()).Returns(Result.Success(id));

        var result = await handler.Handle(new DownloadWebPageCommand(id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _service.Received().DownloadWebPageAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DownloadWebPagesHandler_DelegatesToService()
    {
        var handler = new DownloadWebPagesHandler(_service);
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        _service.DownloadWebPagesAsync(ids, Arg.Any<CancellationToken>()).Returns(new[] { Result.Success(ids[0]), Result.Success(ids[1]) });

        var result = await handler.Handle(new DownloadWebPagesCommand(ids), CancellationToken.None);

        result.Count().ShouldBe(2);
        await _service.Received().DownloadWebPagesAsync(ids, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RetryDownloadHandler_DelegatesToService()
    {
        var handler = new RetryDownloadHandler(_service);
        var id = Guid.NewGuid();
        _service.RetryDownloadAsync(id, Arg.Any<CancellationToken>()).Returns(Result.Success(id));

        var result = await handler.Handle(new RetryDownloadCommand(id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _service.Received().RetryDownloadAsync(id, Arg.Any<CancellationToken>());
    }
}

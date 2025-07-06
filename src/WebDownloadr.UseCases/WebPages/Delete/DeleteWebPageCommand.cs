using System.Windows.Input;

namespace WebDownloadr.UseCases.WebPages.Delete;

public record DeleteWebPageCommand(Guid Id) : ICommand<Result>;

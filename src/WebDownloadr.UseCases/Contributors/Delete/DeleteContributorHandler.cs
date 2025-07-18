﻿using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.Contributors.Delete;

/// <summary>
/// Handles <see cref="DeleteContributorCommand"/> by delegating deletion to the service.
/// </summary>
public class DeleteContributorHandler(IDeleteContributorService _deleteContributorService)
  : ICommandHandler<DeleteContributorCommand, Result>
{
  /// <summary>
  /// Deletes a contributor by delegating to the domain service.
  /// </summary>
  /// <param name="request">Command containing the contributor identifier.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Result returned by the delete service.</returns>
  public async Task<Result> Handle(DeleteContributorCommand request, CancellationToken cancellationToken) =>
    // This Approach: Keep Domain Events in the Domain Model / Core project; this becomes a pass-through
    // This is @ardalis's preferred approach
    await _deleteContributorService.DeleteContributor(request.ContributorId);
  // Another Approach: Do the real work here including dispatching domain events - change the event from internal to public
  // @ardalis prefers using the service above so that **domain** event behavior remains in the **domain model** (core project)
  // var aggregateToDelete = await _repository.GetByIdAsync(request.ContributorId);
  // if (aggregateToDelete == null) return Result.NotFound();
  // await _repository.DeleteAsync(aggregateToDelete);
  // var domainEvent = new ContributorDeletedEvent(request.ContributorId);
  // await _mediator.Publish(domainEvent);// return Result.Success();
}

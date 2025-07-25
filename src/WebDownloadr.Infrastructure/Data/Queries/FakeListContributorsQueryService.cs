﻿using WebDownloadr.UseCases.Contributors;
using WebDownloadr.UseCases.Contributors.List;

namespace WebDownloadr.Infrastructure.Data.Queries;

public class FakeListContributorsQueryService : IListContributorsQueryService
{
  public Task<IEnumerable<ContributorDTO>> ListAsync(CancellationToken ct)
  {
    IEnumerable<ContributorDTO> result =
        [new ContributorDTO(1, "Fake Contributor 1", ""),
        new ContributorDTO(2, "Fake Contributor 2", "")];

    return Task.FromResult(result);
  }
}

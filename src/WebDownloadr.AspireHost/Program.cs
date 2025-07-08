var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebDownloadr_Web>("web");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blog>("blog");

builder.Build().Run();

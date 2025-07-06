using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("DefaultConnection");

builder.AddProject<Projects.Api>("api")
                      .WithReference(connectionString);

builder.Build().Run();

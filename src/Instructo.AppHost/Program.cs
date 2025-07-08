using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("sql").AddDatabase("db");
builder.AddProject<Projects.Api>("api")
                      .WaitFor(db)
                      .WithReference(db);

builder.Build().Run();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);
Environment.SetEnvironmentVariable("USING_ASPIRE", "TRUE");
var db = builder.AddSqlServer("sql").WithDataVolume().AddDatabase("db");
builder.AddProject<Api>("api")
    .WaitFor(db)
    .WithReference(db);

builder.Build().Run();
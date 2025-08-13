var builder = DistributedApplication.CreateBuilder(args);


var sql = builder.AddSqlServer("sql");
var database = sql.AddDatabase("BlazorAspireAppDb");

var cache = builder.AddRedis("cache");


var apiService = builder.AddProject<Projects.BlazorAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.BlazorAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
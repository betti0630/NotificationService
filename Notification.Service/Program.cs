using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

using Notification.Application;
using Notification.Infrastructure;
using Notification.Infrastructure.Models;
using Notification.Infrastructure.Services;
using Notification.Persistence;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction() || builder.Environment.IsEnvironment("Docker"))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Az 8080-as portot a gRPC fogja használni HTTP/2-n
        options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http2);

        // Az 8081-es porton a REST API (Swagger, controller, health, stb.)
        options.ListenAnyIP(8081, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    });
}
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Notification API";
    config.Version = "v1";

    // Security Definition (Bearer JWT)
    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    // Security Requirement (apply globally)
    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
});
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseGrpcWeb();
app.MapGrpcService<NotificationService>().EnableGrpcWeb().RequireCors();
app.MapControllers();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // Add OpenAPI 3.0 document serving middleware
    // Available at: http://localhost:<port>/swagger/v1/swagger.json
    app.UseOpenApi();

    // Add web UIs to interact with the document
    // Available at: http://localhost:<port>/swagger
    app.UseSwaggerUi();

    // Add ReDoc UI to interact with the document
    // Available at: http://localhost:<port>/redoc
    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });
}
app.MapGet("/", () => Results.Redirect("/swagger"));


app.Services.RunDatabaseMigrations();

app.Run();

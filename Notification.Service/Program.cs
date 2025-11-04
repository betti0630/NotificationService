using Notification.Infrastructure.Models;

using Microsoft.Extensions.Configuration;

using Notification.Application;
using Notification.Infrastructure;
using Notification.Infrastructure.Services;
using Notification.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));

var app = builder.Build();

app.Services.RunDatabaseMigrations();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseGrpcWeb();
app.MapGrpcService<NotificationService>().EnableGrpcWeb().RequireCors();
app.MapGet("/", () => "Notification gRPC service");
app.Run();

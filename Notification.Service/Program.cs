using Notification.Infrastructure.Models;

using Microsoft.Extensions.Configuration;

using Notification.Application;
using Notification.Infrastructure;
using Notification.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
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

// Configure the HTTP request pipeline.
app.UseCors();
app.UseGrpcWeb();
app.MapGrpcService<NotificationService>().EnableGrpcWeb().RequireCors();
app.MapGet("/", () => "Notification gRPC service");
app.Run();

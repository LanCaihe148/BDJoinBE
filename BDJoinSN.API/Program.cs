using BDJoinSN.API.Extensions;
using BDJoinSN.API.Middleware;
using BDJoinSN.Application;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Identity;
using BDJoinSN.Identity.Features.Auth.Commands.ChangePassword;
using BDJoinSN.Identity.Seed;
using BDJoinSN.Infrastructure;
using BDJoinSN.Infrastructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

// Puerto para Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Tus servicios
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);
builder.Services.AddScoped<IProfileCreationService, ProfileCreationService>();
builder.Services.AddScoped<IRequestHandler<ChangePasswordCommand, bool>, ChangePasswordHandler>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
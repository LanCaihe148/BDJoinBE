using BDJoinSN.API.Extensions;
using BDJoinSN.API.Middleware;
using BDJoinSN.Application;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Identity;
using BDJoinSN.Identity.Features.Auth.Commands.ChangePassword;
using BDJoinSN.Infrastructure;
using BDJoinSN.Infrastructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    if (context.HostingEnvironment.IsProduction())
    {
        // Usa el método alternativo que no usa FileSystemWatcher
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
    }
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
// Health Checks
builder.Services.AddHealthChecks();

// Servicios de aplicación
builder.Services.AddApplicationServices();

// Servicios de infraestructura
builder.Services.AddInfrastructureServices(builder.Configuration);

// Servicios de Identity
builder.Services.ConfigureIdentityServices(builder.Configuration);

// Registrar servicios adicionales
builder.Services.AddScoped<IProfileCreationService, ProfileCreationService>();
builder.Services.AddScoped<IRequestHandler<ChangePasswordCommand, bool>, ChangePasswordHandler>();

// Configurar Swagger
builder.Services.ConfigureSwaggerServices();

// ============================================
// 3. CONFIGURAR CORS (PERMITIR TODO EN RENDER)
// ============================================
builder.Services.AddCors(op =>
    op.AddPolicy("CorsPolicy",
    policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    }));

// ============================================
// 4. CONTROLLERS Y OPENAPI
// ============================================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ============================================
// 5. BUILD APP
// ============================================
var app = builder.Build();

// ============================================
// 6. MIDDLEWARE PIPELINE
// ============================================

// Swagger en desarrollo (o siempre)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // En producción, habilitar Swagger para documentación
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BDJoinSN API v1");
    });
}

// Exception Middleware (SIEMPRE)
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TokenBlacklistMiddleware>();
// Swagger Services
app.UseSwaggerServices();

// Health Check
app.MapHealthChecks("/healthz");

// CORS
app.UseCors("CorsPolicy");

// Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// ============================================
// 7. RUN
// ============================================
app.Run();
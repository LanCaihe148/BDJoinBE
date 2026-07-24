using BDJoinSN.API.Extensions;
using BDJoinSN.API.Middleware;
using BDJoinSN.Application;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Identity;
using BDJoinSN.Infrastructure;
using BDJoinSN.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHealthChecks();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);
builder.Services.AddScoped<IProfileCreationService, ProfileCreationService>();
builder.Services.ConfigureSwaggerServices();
builder.Services.AddCors(op =>
    op.AddPolicy("CorsPolicy",
    policy =>
    {
        policy.WithOrigins()
        .AllowAnyMethod()
        .AllowAnyHeader();
    }));


builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwaggerServices();

app.MapHealthChecks("/healthz");

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

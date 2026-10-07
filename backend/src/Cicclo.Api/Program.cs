using System.Text.Json.Serialization;
using Cicclo.Api.ExceptionHandling;
using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Application.Dtos;
using Cicclo.Application.Queries;
using Cicclo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Enums como texto no JSON (ex.: "Wash" em vez de 1).
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// Repositórios singleton: os dados ficam em memória enquanto a API estiver rodando.
builder.Services.AddSingleton<ILaundryServiceRepository, InMemoryLaundryServiceRepository>();
builder.Services.AddScoped<IQueryHandler<ListServicesQuery, IReadOnlyList<ServiceDto>>, ListServicesQueryHandler>();

builder.Services.AddSingleton<IWalletRepository, InMemoryWalletRepository>();
builder.Services.AddScoped<IQueryHandler<GetWalletQuery, WalletDto>, GetWalletQueryHandler>();

builder.Services.AddSingleton<IServiceExecutionRepository, InMemoryServiceExecutionRepository>();
// Singleton: o handler mantém o lock que torna o débito atômico.
builder.Services.AddSingleton<ICommandHandler<ExecuteServiceCommand, ServiceExecutionDto>, ExecuteServiceCommandHandler>();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("HealthCheck");

app.MapControllers();

app.Run();

public partial class Program;

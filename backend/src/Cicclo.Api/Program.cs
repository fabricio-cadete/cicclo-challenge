using System.Text.Json.Serialization;
using Cicclo.Api;
using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Application.Dtos;
using Cicclo.Application.Queries;
using Cicclo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Enums como texto no JSON (ex.: "Wash" em vez de 1).
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Singleton: os dados ficam em memória enquanto a API estiver rodando.
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

app.MapGet("/services", async (IQueryHandler<ListServicesQuery, IReadOnlyList<ServiceDto>> handler, CancellationToken ct) =>
        Results.Ok(await handler.Handle(new ListServicesQuery(), ct)))
    .WithName("ListServices");

app.MapGet("/wallet", async (IQueryHandler<GetWalletQuery, WalletDto> handler, CancellationToken ct) =>
        Results.Ok(await handler.Handle(new GetWalletQuery(), ct)))
    .WithName("GetWallet");

app.MapPost("/services/{serviceId:guid}/executions",
        async (Guid serviceId, ExecuteServiceRequest body,
            ICommandHandler<ExecuteServiceCommand, ServiceExecutionDto> handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new ExecuteServiceCommand(serviceId, body.RequestId), ct)))
    .WithName("ExecuteService");

app.Run();

public record ExecuteServiceRequest(Guid RequestId);

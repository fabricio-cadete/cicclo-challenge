using System.Text.Json.Serialization;
using Cicclo.Application.Abstractions;
using Cicclo.Application.Queries;
using Cicclo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Enums como texto no JSON (ex.: "Wash" em vez de 1).
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Singleton: os dados ficam em memória enquanto a API estiver rodando.
builder.Services.AddSingleton<ILaundryServiceRepository, InMemoryLaundryServiceRepository>();
builder.Services.AddScoped<ListServicesQuery>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("HealthCheck");

app.MapGet("/services", (ListServicesQuery query) => Results.Ok(query.Handle()))
    .WithName("ListServices");

app.Run();

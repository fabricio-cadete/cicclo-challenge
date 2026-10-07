using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cicclo.Api.Tests;

// Cada teste sobe sua própria API: o xUnit cria uma instância da classe por teste,
// então os dados em memória (carteira e execuções) nunca vazam entre testes.
public class ApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public ApiTests() => _client = _factory.CreateClient();

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<JsonElement> GetJsonAsync(string url)
    {
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private async Task<Guid> GetServiceIdAsync(string name)
    {
        var services = await GetJsonAsync("/services");

        return services.EnumerateArray()
            .Single(s => s.GetProperty("name").GetString() == name)
            .GetProperty("id").GetGuid();
    }

    private async Task<decimal> GetBalanceAsync() =>
        (await GetJsonAsync("/wallet")).GetProperty("balance").GetDecimal();

    private Task<HttpResponseMessage> ExecuteAsync(Guid serviceId, Guid? requestId = null) =>
        _client.PostAsJsonAsync($"/services/{serviceId}/execute", new { requestId = requestId ?? Guid.NewGuid() });

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var body = await GetJsonAsync("/health");

        Assert.Equal("Healthy", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task GetServices_ReturnsWashAndDryWithPrices()
    {
        var response = await _client.GetAsync("/services");
        var services = (await response.Content.ReadFromJsonAsync<JsonElement>()).EnumerateArray().ToList();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, services.Count);

        var wash = services.Single(s => s.GetProperty("name").GetString() == "Lavagem");
        Assert.Equal(18.90m, wash.GetProperty("price").GetDecimal());
        Assert.Equal("Wash", wash.GetProperty("type").GetString());

        var dry = services.Single(s => s.GetProperty("name").GetString() == "Secagem");
        Assert.Equal(20.90m, dry.GetProperty("price").GetDecimal());
        Assert.Equal("Dry", dry.GetProperty("type").GetString());
    }

    [Fact]
    public async Task GetWallet_StartsWithInitialBalance()
    {
        Assert.Equal(50.00m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_ReturnsExecutionAndDebitsWallet()
    {
        var washId = await GetServiceIdAsync("Lavagem");

        var response = await ExecuteAsync(washId);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(washId, body.GetProperty("serviceId").GetGuid());
        Assert.Equal(18.90m, body.GetProperty("price").GetDecimal());
        Assert.Equal("Requested", body.GetProperty("status").GetString());
        Assert.Equal(31.10m, body.GetProperty("walletBalance").GetDecimal());
        Assert.Equal(31.10m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_WashThenDry_AccumulatesDebits()
    {
        await ExecuteAsync(await GetServiceIdAsync("Lavagem"));
        await ExecuteAsync(await GetServiceIdAsync("Secagem"));

        // 50,00 - 18,90 - 20,90
        Assert.Equal(10.20m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_SameRequestIdTwice_ChargesOnce()
    {
        var washId = await GetServiceIdAsync("Lavagem");
        var requestId = Guid.NewGuid();

        var first = await ExecuteAsync(washId, requestId);
        var second = await ExecuteAsync(washId, requestId);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstId = (await first.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        var secondId = (await second.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        Assert.Equal(firstId, secondId);
        Assert.Equal(31.10m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_SameRequestIdForAnotherService_ReturnsConflict()
    {
        var requestId = Guid.NewGuid();
        await ExecuteAsync(await GetServiceIdAsync("Lavagem"), requestId);

        var response = await ExecuteAsync(await GetServiceIdAsync("Secagem"), requestId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(31.10m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_UnknownService_ReturnsNotFoundAndKeepsBalance()
    {
        var response = await ExecuteAsync(Guid.NewGuid());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(50.00m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_InsufficientBalance_ReturnsUnprocessableAndKeepsBalance()
    {
        var washId = await GetServiceIdAsync("Lavagem");
        await ExecuteAsync(washId);
        await ExecuteAsync(washId);

        // saldo restante: 12,20 (< 18,90)
        var response = await ExecuteAsync(washId);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("Saldo insuficiente", problem.GetProperty("title").GetString());
        Assert.Equal(12.20m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_EmptyRequestId_ReturnsBadRequestAndKeepsBalance()
    {
        var response = await ExecuteAsync(await GetServiceIdAsync("Lavagem"), Guid.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(50.00m, await GetBalanceAsync());
    }

    [Fact]
    public async Task Execute_ConcurrentRequests_NeverOverdrawsWallet()
    {
        var washId = await GetServiceIdAsync("Lavagem");

        var responses = await Task.WhenAll(
            Enumerable.Range(0, 10).Select(_ => Task.Run(() => ExecuteAsync(washId))));

        // 50,00 comporta só 2 lavagens de 18,90
        Assert.Equal(2, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(8, responses.Count(r => r.StatusCode == HttpStatusCode.UnprocessableEntity));
        Assert.Equal(12.20m, await GetBalanceAsync());
    }
}

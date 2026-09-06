using System.Net.Http.Json;
using Web.Models.Api;

namespace Web.Services.Api;

public class MaktabAhvazApiClient
{
    private readonly HttpClient _httpClient;

    public MaktabAhvazApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HomeApiResponse?> GetHomeAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<HomeApiResponse>(
            "api/home",
            cancellationToken);
    }
}
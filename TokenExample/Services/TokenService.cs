using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _tokenEndpoint = "https://localhost:7196/token";
    private readonly string _clientId = "your-client-id";
    private readonly string _clientSecret = "your-client-secret";

    public TokenService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<string> GetTokenAsync()
    {
        if (!_cache.TryGetValue("access_token", out string accessToken))
        {
            var tokenResponse = await RequestTokenAsync();
            accessToken = tokenResponse.access_token;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.expires_in - 60)); 

            _cache.Set("access_token", accessToken, cacheEntryOptions);
        }

        return accessToken;
    }

    private async Task<TokenResponse> RequestTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse;
    }
}

public class TokenResponse
{
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string access_token { get; set; }
}

public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;
    private readonly string _orderEndpoint = "https://localhost:7196/orders";

    public OrderService(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
    }

    public async Task FetchOrdersAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(_orderEndpoint);
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadAsStringAsync();
        Console.WriteLine(orders);
    }
}

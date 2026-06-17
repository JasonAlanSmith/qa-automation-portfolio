using System.Net;
using System.Text;
using System.Text.Json;

namespace MaelstromOps.Playwright;

/// <summary>
/// HTTP client for the hybrid pattern: authenticate and manage test data via the
/// API. A CookieContainer captures the session cookies (incl. HttpOnly values),
/// which are injected into the browser context for fast, login-free auth.
/// </summary>
public class ApiClient
{
    private readonly CookieContainer _cookies = new();
    private readonly HttpClient _http;

    public ApiClient()
    {
        var handler = new HttpClientHandler { CookieContainer = _cookies, UseCookies = true };
        _http = new HttpClient(handler);
    }

    public async Task<List<(string Name, string Value)>> LoginAsync()
    {
        var res = await PostJsonAsync("authentication/login",
            new { email = Config.Email, password = Config.Password });
        if ((int)res.StatusCode != 200)
            throw new Exception($"API login failed: {(int)res.StatusCode}");
        return _cookies.GetCookies(new Uri(Config.ApiBaseUrl))
            .Cast<Cookie>().Select(c => (c.Name, c.Value)).ToList();
    }

    public async Task<(string SysId, string Name)> SeedCustomerAsync(string name)
    {
        var kind = await FirstSysIdAsync("references/customer-kinds");
        var theme = await FirstSysIdAsync("references/customer-themes");
        var res = await PostJsonAsync("customers",
            new { name, isIndividual = false, isActive = true, kindSysId = kind, themeSysId = theme });
        var body = await res.Content.ReadAsStringAsync();
        if ((int)res.StatusCode != 201)
            throw new Exception($"seedCustomer failed: {(int)res.StatusCode} {body}");
        var sysId = JsonDocument.Parse(body).RootElement.GetProperty("sysId").GetString()!;
        return (sysId, name);
    }

    public async Task<int> DeleteCustomerAsync(string sysId)
    {
        var res = await _http.DeleteAsync($"{Config.ApiBaseUrl}/customers/{sysId}");
        return (int)res.StatusCode;
    }

    public async Task<int> CustomerStatusAsync(string sysId)
    {
        var res = await _http.GetAsync($"{Config.ApiBaseUrl}/customers/{sysId}");
        return (int)res.StatusCode;
    }

    private async Task<string> FirstSysIdAsync(string path)
    {
        var res = await _http.GetAsync($"{Config.ApiBaseUrl}/{path}");
        var body = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body).RootElement[0].GetProperty("sysId").GetString()!;
    }

    private Task<HttpResponseMessage> PostJsonAsync(string path, object payload)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return _http.PostAsync($"{Config.ApiBaseUrl}/{path}", content);
    }
}

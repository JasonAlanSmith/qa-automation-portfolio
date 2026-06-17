using System.Net;
using System.Text;
using System.Text.Json;

namespace MaelstromOps.SeleniumTests;

/// <summary>
/// Synchronous HTTP client for the hybrid pattern: authenticate and manage test
/// data via the API. A CookieContainer captures the session cookies (incl.
/// HttpOnly values) for injection into the browser.
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

    public List<(string Name, string Value)> Login()
    {
        var res = PostJson("authentication/login",
            new { email = Config.Email, password = Config.Password });
        if ((int)res.StatusCode != 200)
            throw new Exception($"API login failed: {(int)res.StatusCode}");
        return _cookies.GetCookies(new Uri(Config.ApiBaseUrl))
            .Cast<Cookie>().Select(c => (c.Name, c.Value)).ToList();
    }

    public (string SysId, string Name) SeedCustomer(string name)
    {
        var kind = FirstSysId("references/customer-kinds");
        var theme = FirstSysId("references/customer-themes");
        var res = PostJson("customers",
            new { name, isIndividual = false, isActive = true, kindSysId = kind, themeSysId = theme });
        var body = Read(res);
        if ((int)res.StatusCode != 201)
            throw new Exception($"seedCustomer failed: {(int)res.StatusCode} {body}");
        var sysId = JsonDocument.Parse(body).RootElement.GetProperty("sysId").GetString()!;
        return (sysId, name);
    }

    public int DeleteCustomer(string sysId) =>
        (int)_http.DeleteAsync($"{Config.ApiBaseUrl}/customers/{sysId}")
            .GetAwaiter().GetResult().StatusCode;

    public int CustomerStatus(string sysId) =>
        (int)_http.GetAsync($"{Config.ApiBaseUrl}/customers/{sysId}")
            .GetAwaiter().GetResult().StatusCode;

    private string FirstSysId(string path)
    {
        var res = _http.GetAsync($"{Config.ApiBaseUrl}/{path}").GetAwaiter().GetResult();
        return JsonDocument.Parse(Read(res)).RootElement[0].GetProperty("sysId").GetString()!;
    }

    private HttpResponseMessage PostJson(string path, object payload)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return _http.PostAsync($"{Config.ApiBaseUrl}/{path}", content).GetAwaiter().GetResult();
    }

    private static string Read(HttpResponseMessage res) =>
        res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
}

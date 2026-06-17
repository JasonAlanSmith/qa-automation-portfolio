using Microsoft.Playwright;
using NUnit.Framework;

namespace MaelstromOps.Playwright;

public abstract class BaseTest
{
    protected IPlaywright Pw = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    // One API client + one login for the whole run (the 15-minute access token
    // covers it), shared across tests — fast, and never trips the auth rate-limit.
    protected static readonly ApiClient Api = new();
    private static List<(string Name, string Value)>? _sessionCookies;

    [SetUp]
    public async Task SetUpAsync()
    {
        Pw = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Pw.Chromium.LaunchAsync(new() { Headless = true });
        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await Context.CloseAsync();
        await Browser.CloseAsync();
        Pw.Dispose();
    }

    /// <summary>Inject the (once-acquired) session cookies into this test's context.</summary>
    protected async Task AuthenticateAsync()
    {
        _sessionCookies ??= await Api.LoginAsync();
        var host = new Uri(Config.BaseUrl).Host;
        await Context.AddCookiesAsync(_sessionCookies.Select(c => new Cookie
        {
            Name = c.Name,
            Value = c.Value,
            Domain = host,
            Path = "/",
        }));
    }

    protected static string UniqueName(string prefix) =>
        $"{prefix}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Random.Shared.Next(10000)}";
}

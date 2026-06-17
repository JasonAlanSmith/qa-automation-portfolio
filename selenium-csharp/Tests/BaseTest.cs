using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MaelstromOps.SeleniumTests;

public abstract class BaseTest
{
    protected IWebDriver Driver = null!;

    // One API client + one login for the whole run, shared across tests (the
    // 15-minute access token covers it) — fast, and never trips the auth rate-limit.
    protected static readonly ApiClient Api = new();
    private static List<(string Name, string Value)>? _sessionCookies;

    [SetUp]
    public void CreateDriver() => Driver = DriverFactory.Create();

    [TearDown]
    public void QuitDriver() => Driver?.Quit();

    /// <summary>Inject the (once-acquired) session cookies into this test's browser.</summary>
    protected void Authenticate()
    {
        _sessionCookies ??= Api.Login();
        Driver.Navigate().GoToUrl(Config.BaseUrl); // load the origin so cookies stick
        var host = new Uri(Config.BaseUrl).Host;
        foreach (var (name, value) in _sessionCookies)
            Driver.Manage().Cookies.AddCookie(new Cookie(name, value, host, "/", null));
    }

    protected void WaitForUrl(string pattern) =>
        new WebDriverWait(Driver, TimeSpan.FromSeconds(15)).Until(d => Regex.IsMatch(d.Url, pattern));

    protected static string UniqueName(string prefix) =>
        $"{prefix}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Random.Shared.Next(10000)}";
}

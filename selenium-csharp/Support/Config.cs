namespace MaelstromOps.SeleniumTests;

/// <summary>Environment-driven settings — no hard-coded URLs or credentials.</summary>
public static class Config
{
    public static string BaseUrl => Env("BASE_URL", "http://localhost:5173");
    public static string ApiBaseUrl => Env("MOPS_API_BASE_URL", "http://localhost:5009/api/v1");
    public static string Email => Env("MOPS_TEST_EMAIL", "test@test.org");
    public static string Password => Env("MOPS_TEST_PASSWORD", "test");

    /// <summary>Optional explicit Chrome/Chromium binary (Selenium Manager finds the driver).</summary>
    public static string ChromeBinary => Env("CHROME_BINARY", "");

    private static string Env(string key, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}

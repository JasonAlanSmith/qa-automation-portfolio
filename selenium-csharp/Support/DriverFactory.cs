using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MaelstromOps.SeleniumTests;

/// <summary>
/// Builds the WebDriver. Selenium Manager (built into Selenium 4) resolves the
/// matching driver automatically. Explicit waits only — implicit wait is zero.
/// </summary>
public static class DriverFactory
{
    public static IWebDriver Create()
    {
        var options = new ChromeOptions();
        options.AddArguments(
            "--headless=new", "--no-sandbox", "--disable-dev-shm-usage", "--window-size=1400,1000");

        if (!string.IsNullOrWhiteSpace(Config.ChromeBinary))
            options.BinaryLocation = Config.ChromeBinary;

        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        return driver;
    }
}

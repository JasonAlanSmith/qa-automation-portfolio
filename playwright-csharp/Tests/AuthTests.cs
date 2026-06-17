using NUnit.Framework;
using static Microsoft.Playwright.Assertions;

namespace MaelstromOps.Playwright;

/// <summary>Login UI behaviour — runs unauthenticated (no cookie injection).</summary>
[TestFixture]
public class AuthTests : BaseTest
{
    [Test]
    public async Task RejectsWrongPasswordAndStaysOnLogin()
    {
        var login = new LoginPage(Page);
        await login.GotoAsync();
        await login.LoginAsync(Config.Email, "definitely-wrong");

        // No navigation to /home on failure; the login surface remains.
        // (Finding: the UI shows no inline error on a rejected login.)
        await Expect(login.Root).ToBeVisibleAsync();
        Assert.That(Page.Url, Does.Not.Contain("/home"));
    }

    [Test]
    public async Task RejectsUnknownUser()
    {
        var login = new LoginPage(Page);
        await login.GotoAsync();
        await login.LoginAsync("nobody@nowhere.test", "whatever-password");

        await Expect(login.Root).ToBeVisibleAsync();
        Assert.That(Page.Url, Does.Not.Contain("/home"));
    }
}

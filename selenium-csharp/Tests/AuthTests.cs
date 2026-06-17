using NUnit.Framework;

namespace MaelstromOps.SeleniumTests;

/// <summary>Login UI behaviour — runs unauthenticated.</summary>
[TestFixture]
public class AuthTests : BaseTest
{
    [Test]
    public void RejectsWrongPasswordAndStaysOnLogin()
    {
        var login = new LoginPage(Driver).Open();
        login.Login(Config.Email, "definitely-wrong");

        // No navigation to /home on failure; the login surface remains.
        // (Finding: the UI shows no inline error on a rejected login.)
        Assert.That(login.IsLoaded(), Is.True);
        Assert.That(Driver.Url, Does.Not.Contain("/home"));
    }

    [Test]
    public void RejectsUnknownUser()
    {
        var login = new LoginPage(Driver).Open();
        login.Login("nobody@nowhere.test", "whatever-password");

        Assert.That(login.IsLoaded(), Is.True);
        Assert.That(Driver.Url, Does.Not.Contain("/home"));
    }
}

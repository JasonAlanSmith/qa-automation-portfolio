using NUnit.Framework;

namespace MaelstromOps.SeleniumTests;

/// <summary>Authenticated smoke — session injected via the API (no UI login).</summary>
[TestFixture]
public class SmokeTests : BaseTest
{
    [SetUp]
    public void SignIn() => Authenticate();

    [Test]
    public void HomeLoads()
    {
        var home = new HomePage(Driver).Open();
        Assert.That(home.IsLoaded(), Is.True);
        Assert.That(Driver.Url, Does.Not.Contain("/login"));
    }

    [Test]
    public void CustomersGridLoads()
    {
        var customers = new CustomersPage(Driver).Open();
        Assert.That(customers.IsLoaded(), Is.True);
    }
}

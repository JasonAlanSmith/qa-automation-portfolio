using NUnit.Framework;
using static Microsoft.Playwright.Assertions;

namespace MaelstromOps.Playwright;

/// <summary>Authenticated smoke — session injected via the API (no UI login).</summary>
[TestFixture]
public class SmokeTests : BaseTest
{
    [SetUp]
    public Task SignInAsync() => AuthenticateAsync();

    [Test]
    public async Task HomeLoads()
    {
        var home = new HomePage(Page);
        await home.GotoAsync();
        await Expect(home.Root).ToBeVisibleAsync();
        Assert.That(Page.Url, Does.Not.Contain("/login"));
    }

    [Test]
    public async Task CustomersGridLoads()
    {
        var customers = new CustomersPage(Page);
        await customers.GotoAsync();
        await Expect(customers.Root).ToBeVisibleAsync();
    }
}

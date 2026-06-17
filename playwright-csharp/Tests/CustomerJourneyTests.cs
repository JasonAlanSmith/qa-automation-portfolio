using System.Text.RegularExpressions;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;

namespace MaelstromOps.Playwright;

/// <summary>The full customer journey through the UI.</summary>
[TestFixture]
public class CustomerJourneyTests : BaseTest
{
    private static readonly Regex ProfileUrl = new(@"/customer/browse/[0-9a-f-]{36}/profile");

    [SetUp]
    public Task SignInAsync() => AuthenticateAsync();

    [Test]
    public async Task CreatesThroughFormAndLandsOnProfile()
    {
        var name = UniqueName("pwcs-customer");
        var form = new CustomerNewPage(Page);
        await form.GotoAsync();
        await form.CreateOrganizationAsync(name);

        await Page.WaitForURLAsync(ProfileUrl);
        await Expect(new CustomerProfilePage(Page).Root).ToBeVisibleAsync();

        var sysId = Regex.Match(Page.Url, @"browse/([0-9a-f-]{36})/profile").Groups[1].Value;
        await Api.DeleteCustomerAsync(sysId);
    }

    [Test]
    public async Task SeededCustomerRendersInUi()
    {
        var customer = await Api.SeedCustomerAsync(UniqueName("pwcs-seeded"));
        try
        {
            var profile = new CustomerProfilePage(Page);
            await profile.GotoAsync(customer.SysId);
            await Expect(profile.Root).ToContainTextAsync(customer.Name);
        }
        finally
        {
            await Api.DeleteCustomerAsync(customer.SysId);
        }
    }

    [Test]
    public async Task OpensCustomerFromGrid()
    {
        var customers = new CustomersPage(Page);
        await customers.GotoAsync();
        await customers.OpenFirstProfileAsync();

        await Page.WaitForURLAsync(ProfileUrl);
        await Expect(new CustomerProfilePage(Page).Root).ToBeVisibleAsync();
    }

    [Test]
    public async Task EditsThenDeletesThroughUi()
    {
        var customer = await Api.SeedCustomerAsync(UniqueName("pwcs-edit"));
        var deletedInUi = false;
        try
        {
            var profile = new CustomerProfilePage(Page);
            await profile.GotoAsync(customer.SysId);

            var newName = UniqueName("pwcs-renamed");
            await profile.RenameAsync(newName);
            await Expect(profile.Root).ToContainTextAsync(newName);

            await profile.DeleteAsync();
            await Page.WaitForURLAsync(new Regex(@"/customer/browse$"));
            deletedInUi = true;

            Assert.That(await Api.CustomerStatusAsync(customer.SysId), Is.EqualTo(404));
        }
        finally
        {
            if (!deletedInUi)
                await Api.DeleteCustomerAsync(customer.SysId);
        }
    }
}

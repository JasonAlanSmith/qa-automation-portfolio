using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MaelstromOps.SeleniumTests;

/// <summary>The full customer journey through the UI.</summary>
[TestFixture]
public class CustomerJourneyTests : BaseTest
{
    private const string ProfileUrl = "browse/[0-9a-f-]{36}/profile";

    [SetUp]
    public void SignIn() => Authenticate();

    [Test]
    public void CreatesThroughFormAndLandsOnProfile()
    {
        var name = UniqueName("secs-customer");
        new CustomerNewPage(Driver).Open().CreateOrganization(name);

        WaitForUrl(ProfileUrl);
        Assert.That(new CustomerProfilePage(Driver).IsLoaded(), Is.True);

        var sysId = Regex.Match(Driver.Url, "browse/([0-9a-f-]{36})/profile").Groups[1].Value;
        Api.DeleteCustomer(sysId);
    }

    [Test]
    public void SeededCustomerRendersInUi()
    {
        var customer = Api.SeedCustomer(UniqueName("secs-seeded"));
        try
        {
            var profile = new CustomerProfilePage(Driver).Open(customer.SysId);
            Assert.That(profile.ContainsText(customer.Name), Is.True);
        }
        finally
        {
            Api.DeleteCustomer(customer.SysId);
        }
    }

    [Test]
    public void OpensCustomerFromGrid()
    {
        new CustomersPage(Driver).Open().OpenFirstProfile();

        WaitForUrl(ProfileUrl);
        Assert.That(new CustomerProfilePage(Driver).IsLoaded(), Is.True);
    }

    [Test]
    public void EditsThenDeletesThroughUi()
    {
        var customer = Api.SeedCustomer(UniqueName("secs-edit"));
        var deletedInUi = false;
        try
        {
            var profile = new CustomerProfilePage(Driver).Open(customer.SysId);

            var newName = UniqueName("secs-renamed");
            profile.Rename(newName);
            Assert.That(profile.ContainsText(newName), Is.True);

            profile.Delete();
            WaitForUrl("/customer/browse$");
            deletedInUi = true;

            Assert.That(Api.CustomerStatus(customer.SysId), Is.EqualTo(404));
        }
        finally
        {
            if (!deletedInUi)
                Api.DeleteCustomer(customer.SysId);
        }
    }
}

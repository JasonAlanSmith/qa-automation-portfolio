using Microsoft.Playwright;

namespace MaelstromOps.Playwright;

// Page Objects: each holds its locators and exposes intent; assertions stay in
// the tests. Selectors prefer data-testid, input ids, and accessible roles.

public class LoginPage(IPage page)
{
    public ILocator Root => page.GetByTestId("auth.login");

    public async Task GotoAsync()
    {
        await page.GotoAsync($"{Config.BaseUrl}/login");
        await Root.WaitForAsync();
    }

    public async Task LoginAsync(string email, string password)
    {
        await page.Locator("#email").FillAsync(email);
        await page.Locator("#password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();
    }
}

public class HomePage(IPage page)
{
    public ILocator Root => page.GetByTestId("home");
    public Task GotoAsync() => page.GotoAsync($"{Config.BaseUrl}/home");
}

public class CustomersPage(IPage page)
{
    public ILocator Root => page.GetByTestId("customer.browse");

    public async Task GotoAsync()
    {
        await page.GotoAsync($"{Config.BaseUrl}/customer/browse");
        await Root.WaitForAsync();
    }

    public Task OpenFirstProfileAsync() =>
        page.GetByRole(AriaRole.Button, new() { Name = "View" }).First.ClickAsync();
}

public class CustomerNewPage(IPage page)
{
    public ILocator Root => page.GetByTestId("customer.new");

    public async Task GotoAsync()
    {
        await page.GotoAsync($"{Config.BaseUrl}/customer/new");
        await Root.WaitForAsync();
    }

    public async Task CreateOrganizationAsync(string name)
    {
        await page.Locator("#name").FillAsync(name);
        await PickFirstOptionAsync("kindSysId");
        await PickFirstOptionAsync("themeSysId");
        await page.GetByRole(AriaRole.Button, new() { Name = "Create Customer" }).ClickAsync();
    }

    private async Task PickFirstOptionAsync(string fieldId)
    {
        // Open the Syncfusion dropdown via its wrapper, then commit the first real
        // option with real keyboard events (Playwright's keys are honoured).
        await page.Locator($"span.e-ddl:has(input#{fieldId})").ClickAsync();
        await page.Locator(".e-popup-open .e-list-item").First.WaitForAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await page.Keyboard.PressAsync("Enter");
    }
}

public class CustomerProfilePage(IPage page)
{
    public ILocator Root => page.GetByTestId("customer.profile");

    public async Task GotoAsync(string sysId)
    {
        await page.GotoAsync($"{Config.BaseUrl}/customer/browse/{sysId}/profile");
        await Root.WaitForAsync();
    }

    public async Task RenameAsync(string newName)
    {
        await page.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await page.Locator("#name").FillAsync(newName);
        await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
    }

    public async Task DeleteAsync()
    {
        // Auto-accept the native confirm dialog the Delete button raises.
        page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
    }
}

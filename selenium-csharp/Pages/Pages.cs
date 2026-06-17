using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace MaelstromOps.SeleniumTests;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        Wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    protected IWebElement Visible(By by) => Wait.Until(d =>
    {
        var e = d.FindElement(by);
        return e.Displayed ? e : throw new NoSuchElementException("not visible yet");
    });

    protected IWebElement Clickable(By by) => Wait.Until(d =>
    {
        var e = d.FindElement(by);
        return e.Displayed && e.Enabled ? e : throw new NoSuchElementException("not clickable yet");
    });

    protected void Type(By by, string text)
    {
        var e = Visible(by);
        e.Clear();
        e.SendKeys(text);
    }

    /// <summary>
    /// Click via JS, scrolling to centre first — these long forms have submit
    /// controls below the fold under an overlay where a native click is intercepted.
    /// </summary>
    protected void JsClick(By by)
    {
        var e = Wait.Until(d => d.FindElement(by));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", e);
        js.ExecuteScript("arguments[0].click();", e);
    }

    protected bool IsVisible(By by)
    {
        try { return Visible(by).Displayed; }
        catch (WebDriverTimeoutException) { return false; }
    }
}

public class LoginPage(IWebDriver driver) : BasePage(driver)
{
    private static readonly By Root = By.CssSelector("[data-testid='auth.login']");

    public LoginPage Open()
    {
        Driver.Navigate().GoToUrl($"{Config.BaseUrl}/login");
        Visible(Root);
        return this;
    }

    public void Login(string email, string password)
    {
        Type(By.Id("email"), email);
        Type(By.Id("password"), password);
        Clickable(By.XPath("//button[normalize-space()='Log In']")).Click();
    }

    public bool IsLoaded() => IsVisible(Root);
}

public class HomePage(IWebDriver driver) : BasePage(driver)
{
    private static readonly By Root = By.CssSelector("[data-testid='home']");
    public HomePage Open() { Driver.Navigate().GoToUrl($"{Config.BaseUrl}/home"); return this; }
    public bool IsLoaded() => IsVisible(Root);
}

public class CustomersPage(IWebDriver driver) : BasePage(driver)
{
    private static readonly By Root = By.CssSelector("[data-testid='customer.browse']");

    public CustomersPage Open()
    {
        Driver.Navigate().GoToUrl($"{Config.BaseUrl}/customer/browse");
        Visible(Root);
        return this;
    }

    public bool IsLoaded() => IsVisible(Root);

    public void OpenFirstProfile() =>
        Clickable(By.XPath("(//button[normalize-space()='View'])[1]")).Click();
}

public class CustomerNewPage(IWebDriver driver) : BasePage(driver)
{
    private static readonly By Root = By.CssSelector("[data-testid='customer.new']");

    public CustomerNewPage Open()
    {
        Driver.Navigate().GoToUrl($"{Config.BaseUrl}/customer/new");
        Visible(Root);
        return this;
    }

    public void CreateOrganization(string name)
    {
        Type(By.Id("name"), name);
        PickFirstOption("kindSysId");
        PickFirstOption("themeSysId");
        JsClick(By.XPath("//button[normalize-space()='Create Customer']"));
    }

    private void PickFirstOption(string fieldId)
    {
        // Open the Syncfusion dropdown via its wrapper, then commit the first real
        // option with REAL keyboard events (ArrowDown off the placeholder, Enter).
        Clickable(By.CssSelector($"span.e-ddl:has(input#{fieldId})")).Click();
        Visible(By.CssSelector(".e-popup-open .e-list-item"));
        new Actions(Driver).SendKeys(Keys.Down).SendKeys(Keys.Enter).Perform();
    }
}

public class CustomerProfilePage(IWebDriver driver) : BasePage(driver)
{
    private static readonly By Root = By.CssSelector("[data-testid='customer.profile']");

    public CustomerProfilePage Open(string sysId)
    {
        Driver.Navigate().GoToUrl($"{Config.BaseUrl}/customer/browse/{sysId}/profile");
        Visible(Root);
        return this;
    }

    public bool IsLoaded() => IsVisible(Root);

    public bool ContainsText(string text) =>
        Wait.Until(d => d.FindElement(Root).Text.Contains(text));

    public void Rename(string newName)
    {
        JsClick(By.XPath("//button[normalize-space()='Edit']"));
        Type(By.Id("name"), newName);
        JsClick(By.XPath("//button[normalize-space()='Save']"));
    }

    public void Delete()
    {
        JsClick(By.XPath("//button[normalize-space()='Delete']"));
        Wait.Until(d =>
        {
            try { d.SwitchTo().Alert().Accept(); return true; }
            catch (NoAlertPresentException) { return false; }
        });
    }
}

package com.maelstromops.qa.pages;

import com.maelstromops.qa.support.Config;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;

public class CustomersPage extends BasePage {

  private final By root = By.cssSelector("[data-testid='customer.browse']");
  private final By firstViewButton =
      By.xpath("(//button[normalize-space()='View'])[1]");

  public CustomersPage(WebDriver driver) {
    super(driver);
  }

  public CustomersPage open() {
    driver.get(Config.baseUrl() + "/customer/browse");
    visible(root);
    return this;
  }

  public boolean isLoaded() {
    return isVisible(root);
  }

  /** Click the first row's "View" command — navigates to that customer's profile. */
  public void openFirstProfile() {
    clickable(firstViewButton).click();
  }
}

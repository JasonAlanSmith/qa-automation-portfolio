package com.maelstromops.qa.pages;

import com.maelstromops.qa.support.Config;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.interactions.Actions;

public class CustomerNewPage extends BasePage {

  private final By root = By.cssSelector("[data-testid='customer.new']");
  private final By name = By.id("name");
  private final By submit = By.xpath("//button[normalize-space()='Create Customer']");

  public CustomerNewPage(WebDriver driver) {
    super(driver);
  }

  public CustomerNewPage open() {
    driver.get(Config.baseUrl() + "/customer/new");
    visible(root);
    return this;
  }

  public void createOrganization(String customerName) {
    type(name, customerName);
    pickFirstOption("kindSysId");
    pickFirstOption("themeSysId");
    click(submit);
  }

  private void pickFirstOption(String fieldId) {
    // Open the Syncfusion dropdown via its wrapper (the readonly input intercepts
    // clicks), then commit the first real option with REAL keyboard events:
    // ArrowDown moves off the "-- Select --" placeholder, Enter commits. Selenium's
    // sendKeys dispatches real key events that the widget honours.
    clickable(By.cssSelector("span.e-ddl:has(input#" + fieldId + ")")).click();
    visible(By.cssSelector(".e-popup-open .e-list-item"));
    new Actions(driver).sendKeys(Keys.ARROW_DOWN, Keys.ENTER).perform();
  }
}

package com.maelstromops.qa.pages;

import com.maelstromops.qa.support.Config;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;

public class CustomerProfilePage extends BasePage {

  private final By root = By.cssSelector("[data-testid='customer.profile']");
  private final By name = By.id("name");
  private final By editButton = By.xpath("//button[normalize-space()='Edit']");
  private final By saveButton = By.xpath("//button[normalize-space()='Save']");
  private final By deleteButton = By.xpath("//button[normalize-space()='Delete']");

  public CustomerProfilePage(WebDriver driver) {
    super(driver);
  }

  public CustomerProfilePage open(String sysId) {
    driver.get(Config.baseUrl() + "/customer/browse/" + sysId + "/profile");
    visible(root);
    return this;
  }

  public boolean isLoaded() {
    return isVisible(root);
  }

  public boolean containsText(String text) {
    return wait.until(ExpectedConditions.textToBePresentInElementLocated(root, text));
  }

  /** Enter edit mode, change the name, and save (drops back to the read-only view). */
  public void rename(String newName) {
    click(editButton);
    type(name, newName);
    click(saveButton);
  }

  /** Click Delete and accept the native confirm dialog it raises. */
  public void deleteCustomer() {
    click(deleteButton);
    wait.until(ExpectedConditions.alertIsPresent()).accept();
  }
}

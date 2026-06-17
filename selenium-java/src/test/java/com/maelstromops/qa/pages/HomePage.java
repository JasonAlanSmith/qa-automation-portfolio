package com.maelstromops.qa.pages;

import com.maelstromops.qa.support.Config;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;

public class HomePage extends BasePage {

  private final By root = By.cssSelector("[data-testid='home']");

  public HomePage(WebDriver driver) {
    super(driver);
  }

  public HomePage open() {
    driver.get(Config.baseUrl() + "/home");
    return this;
  }

  public boolean isLoaded() {
    return isVisible(root);
  }
}

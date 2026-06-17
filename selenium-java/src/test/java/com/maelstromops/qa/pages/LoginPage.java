package com.maelstromops.qa.pages;

import com.maelstromops.qa.support.Config;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;

public class LoginPage extends BasePage {

  private final By root = By.cssSelector("[data-testid='auth.login']");
  private final By email = By.id("email");
  private final By password = By.id("password");
  private final By submit = By.xpath("//button[normalize-space()='Log In']");

  public LoginPage(WebDriver driver) {
    super(driver);
  }

  public LoginPage open() {
    driver.get(Config.baseUrl() + "/login");
    visible(root);
    return this;
  }

  public void login(String emailValue, String passwordValue) {
    type(email, emailValue);
    type(password, passwordValue);
    clickable(submit).click();
  }

  public boolean isLoaded() {
    return isVisible(root);
  }
}

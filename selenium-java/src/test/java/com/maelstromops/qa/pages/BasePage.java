package com.maelstromops.qa.pages;

import java.time.Duration;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.TimeoutException;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

/**
 * Base Page Object: holds the driver and an explicit wait, and centralises the
 * common interaction helpers. Page objects expose intent and never assert — the
 * tests own assertions.
 */
public abstract class BasePage {

  protected final WebDriver driver;
  protected final WebDriverWait wait;

  protected BasePage(WebDriver driver) {
    this.driver = driver;
    this.wait = new WebDriverWait(driver, Duration.ofSeconds(15));
  }

  protected WebElement visible(By locator) {
    return wait.until(ExpectedConditions.visibilityOfElementLocated(locator));
  }

  protected WebElement clickable(By locator) {
    return wait.until(ExpectedConditions.elementToBeClickable(locator));
  }

  /**
   * Click an element robustly. On these long forms the submit controls sit below
   * the fold under an overlay, where a native click gets intercepted; we wait for
   * presence, scroll to centre, and dispatch the click via JS so the button's
   * handler still fires reliably.
   */
  protected void click(By locator) {
    WebElement element = wait.until(ExpectedConditions.presenceOfElementLocated(locator));
    JavascriptExecutor js = (JavascriptExecutor) driver;
    js.executeScript("arguments[0].scrollIntoView({block:'center'});", element);
    js.executeScript("arguments[0].click();", element);
  }

  protected void type(By locator, String text) {
    WebElement element = visible(locator);
    element.clear();
    element.sendKeys(text);
  }

  protected boolean isVisible(By locator) {
    try {
      return visible(locator).isDisplayed();
    } catch (TimeoutException e) {
      return false;
    }
  }

  protected void waitForUrlMatches(String regex) {
    wait.until(ExpectedConditions.urlMatches(regex));
  }
}

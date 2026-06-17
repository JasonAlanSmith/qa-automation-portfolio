package com.maelstromops.qa.support;

import java.time.Duration;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;

/**
 * Builds the WebDriver. Selenium Manager (built into Selenium 4) resolves the
 * matching driver automatically, so there's no driver binary to manage. Implicit
 * waits are left at zero — the suite uses explicit waits exclusively.
 */
public final class DriverFactory {

  private DriverFactory() {}

  public static WebDriver create() {
    ChromeOptions options = new ChromeOptions();
    options.addArguments(
        "--headless=new",
        "--no-sandbox",
        "--disable-dev-shm-usage",
        "--window-size=1400,1000");

    String binary = Config.chromeBinary();
    if (!binary.isBlank()) {
      options.setBinary(binary);
    }

    WebDriver driver = new ChromeDriver(options);
    driver.manage().timeouts().implicitlyWait(Duration.ZERO);
    return driver;
  }
}

package com.maelstromops.qa.tests;

import com.maelstromops.qa.support.ApiClient;
import com.maelstromops.qa.support.Config;
import com.maelstromops.qa.support.DriverFactory;
import java.time.Duration;
import java.util.Map;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.openqa.selenium.Cookie;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

/** Driver lifecycle + shared helpers for all tests. */
public abstract class BaseTest {

  // ONE API login for the whole run, shared across tests. The 15-minute access
  // token easily covers a test run, so we authenticate once (like the other
  // suites) — fast, and it never trips the API's per-IP auth rate-limit.
  protected static final ApiClient api = new ApiClient();
  private static Map<String, String> sessionCookies;

  protected WebDriver driver;

  @BeforeEach
  void createDriver() {
    driver = DriverFactory.create();
  }

  @AfterEach
  void quitDriver() {
    if (driver != null) {
      driver.quit();
    }
  }

  /** Inject the (once-acquired) session cookies into this test's fresh browser. */
  protected void authenticate() {
    if (sessionCookies == null) {
      sessionCookies = api.login();
    }
    // Load the app origin first so cookies are accepted for this host.
    driver.get(Config.baseUrl());
    for (Map.Entry<String, String> cookie : sessionCookies.entrySet()) {
      driver.manage().addCookie(
          new Cookie.Builder(cookie.getKey(), cookie.getValue()).path("/").build());
    }
  }

  protected void waitForUrl(String regex) {
    new WebDriverWait(driver, Duration.ofSeconds(15))
        .until(ExpectedConditions.urlMatches(regex));
  }
}

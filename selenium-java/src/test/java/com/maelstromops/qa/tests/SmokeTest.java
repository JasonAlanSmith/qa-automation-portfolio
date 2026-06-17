package com.maelstromops.qa.tests;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.maelstromops.qa.pages.CustomersPage;
import com.maelstromops.qa.pages.HomePage;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

/** Authenticated smoke — session injected via the API (no UI login). */
@DisplayName("authenticated shell")
class SmokeTest extends BaseTest {

  @BeforeEach
  void signIn() {
    authenticate();
  }

  @Test
  @DisplayName("home loads for an authenticated user")
  void homeLoads() {
    HomePage home = new HomePage(driver).open();
    assertTrue(home.isLoaded(), "home surface should be visible");
    assertFalse(driver.getCurrentUrl().contains("/login"), "should not bounce to /login");
  }

  @Test
  @DisplayName("navigates to the Customers browse grid")
  void customersGridLoads() {
    CustomersPage customers = new CustomersPage(driver).open();
    assertTrue(customers.isLoaded());
  }
}

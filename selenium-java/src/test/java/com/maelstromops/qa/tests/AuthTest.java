package com.maelstromops.qa.tests;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.maelstromops.qa.pages.LoginPage;
import com.maelstromops.qa.support.Config;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

/** Login UI behaviour — runs unauthenticated (no cookie injection). */
@DisplayName("login")
class AuthTest extends BaseTest {

  @Test
  @DisplayName("rejects a wrong password and stays on the login page")
  void rejectsWrongPassword() {
    LoginPage login = new LoginPage(driver).open();
    login.login(Config.email(), "definitely-wrong");

    // No navigation to /home on failure; the login surface remains.
    // (Finding: the UI shows no inline error on a rejected login.)
    assertTrue(login.isLoaded(), "login form should remain visible");
    assertFalse(driver.getCurrentUrl().contains("/home"), "should not reach /home");
  }

  @Test
  @DisplayName("rejects an unknown user")
  void rejectsUnknownUser() {
    LoginPage login = new LoginPage(driver).open();
    login.login("nobody@nowhere.test", "whatever-password");

    assertTrue(login.isLoaded());
    assertFalse(driver.getCurrentUrl().contains("/home"));
  }
}

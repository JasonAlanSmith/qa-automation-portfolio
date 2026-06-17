package com.maelstromops.qa.tests;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.maelstromops.qa.pages.CustomerNewPage;
import com.maelstromops.qa.pages.CustomerProfilePage;
import com.maelstromops.qa.pages.CustomersPage;
import java.util.Map;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

/** The full customer journey through the UI. */
@DisplayName("customer journey")
class CustomerJourneyTest extends BaseTest {

  private static final String PROFILE = "browse/[0-9a-f-]{36}/profile";

  @BeforeEach
  void signIn() {
    authenticate();
  }

  private static String uniqueName(String prefix) {
    return prefix + "-" + System.currentTimeMillis() + "-" + (int) (Math.random() * 10000);
  }

  @Test
  @DisplayName("creates a customer through the form and lands on its profile")
  void createThroughFormLandsOnProfile() {
    String name = uniqueName("se-customer");
    new CustomerNewPage(driver).open().createOrganization(name);

    waitForUrl(PROFILE);
    assertTrue(new CustomerProfilePage(driver).isLoaded());

    // Clean up via the API using the id from the resulting URL.
    String sysId = driver.getCurrentUrl()
        .replaceAll(".*/browse/([0-9a-f-]{36})/profile.*", "$1");
    api.deleteCustomer(sysId);
  }

  @Test
  @DisplayName("renders an API-seeded customer in the UI (hybrid)")
  void seededCustomerRendersInUi() {
    Map<String, String> customer = api.seedCustomer(uniqueName("se-seeded"));
    try {
      CustomerProfilePage profile = new CustomerProfilePage(driver).open(customer.get("sysId"));
      assertTrue(profile.isLoaded());
      assertTrue(profile.containsText(customer.get("name")));
    } finally {
      api.deleteCustomer(customer.get("sysId"));
    }
  }

  @Test
  @DisplayName("opens a customer from the grid")
  void openCustomerFromGrid() {
    new CustomersPage(driver).open().openFirstProfile();

    waitForUrl(PROFILE);
    assertTrue(new CustomerProfilePage(driver).isLoaded());
  }

  @Test
  @DisplayName("edits then deletes a customer entirely through the UI")
  void editThenDeleteThroughUi() {
    Map<String, String> customer = api.seedCustomer(uniqueName("se-edit"));
    boolean deletedInUi = false;
    try {
      CustomerProfilePage profile = new CustomerProfilePage(driver).open(customer.get("sysId"));

      String newName = uniqueName("se-renamed");
      profile.rename(newName);
      assertTrue(profile.containsText(newName));

      profile.deleteCustomer();
      waitForUrl("/customer/browse$");
      deletedInUi = true;

      assertEquals(404, api.customerStatus(customer.get("sysId")));
    } finally {
      if (!deletedInUi) {
        api.deleteCustomer(customer.get("sysId"));
      }
    }
  }
}

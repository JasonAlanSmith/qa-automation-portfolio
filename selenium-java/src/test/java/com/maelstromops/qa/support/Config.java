package com.maelstromops.qa.support;

/**
 * Environment-driven settings — never hard-code URLs or credentials in tests.
 * Values come from environment variables (or -D system properties), with
 * sensible local-dev defaults.
 */
public final class Config {

  private Config() {}

  public static String baseUrl() {
    return get("BASE_URL", "http://localhost:5173");
  }

  public static String apiBaseUrl() {
    return get("MOPS_API_BASE_URL", "http://localhost:5009/api/v1");
  }

  public static String email() {
    return get("MOPS_TEST_EMAIL", "test@test.org");
  }

  public static String password() {
    return get("MOPS_TEST_PASSWORD", "test");
  }

  /** Optional explicit Chrome/Chromium binary (Selenium Manager finds the driver). */
  public static String chromeBinary() {
    return get("CHROME_BINARY", "");
  }

  private static String get(String key, String def) {
    String v = System.getenv(key);
    if (v == null || v.isBlank()) {
      v = System.getProperty(key);
    }
    return (v == null || v.isBlank()) ? def : v;
  }
}

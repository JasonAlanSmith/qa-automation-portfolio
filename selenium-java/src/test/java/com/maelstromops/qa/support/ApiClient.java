package com.maelstromops.qa.support;

import com.google.gson.JsonParser;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * A small HTTP client for the hybrid pattern: authenticate and manage test data
 * through the API (fast, reliable), so the browser is reserved for asserting UI
 * behaviour. After {@link #login()} the captured cookies authorise both further
 * API calls here and — once injected into the driver — the browser session.
 */
public class ApiClient {

  private final HttpClient http = HttpClient.newHttpClient();
  private Map<String, String> cookies = new LinkedHashMap<>();

  /** Logs in and returns the session cookies (name -> value). */
  public Map<String, String> login() {
    String body = String.format(
        "{\"email\":\"%s\",\"password\":\"%s\"}", Config.email(), Config.password());
    HttpResponse<String> res = send(HttpRequest.newBuilder(uri("authentication/login"))
        .header("Content-Type", "application/json")
        .POST(HttpRequest.BodyPublishers.ofString(body)));
    if (res.statusCode() != 200) {
      throw new IllegalStateException("API login failed: " + res.statusCode() + " " + res.body());
    }
    Map<String, String> parsed = new LinkedHashMap<>();
    for (String setCookie : res.headers().allValues("set-cookie")) {
      String nameValue = setCookie.split(";", 2)[0];
      int eq = nameValue.indexOf('=');
      if (eq > 0) {
        parsed.put(nameValue.substring(0, eq), nameValue.substring(eq + 1));
      }
    }
    this.cookies = parsed;
    return parsed;
  }

  /** Creates a customer and returns {"sysId":..., "name":...}. */
  public Map<String, String> seedCustomer(String name) {
    String kind = firstSysId(get("references/customer-kinds"));
    String theme = firstSysId(get("references/customer-themes"));
    String body = String.format(
        "{\"name\":\"%s\",\"isIndividual\":false,\"isActive\":true,"
            + "\"kindSysId\":\"%s\",\"themeSysId\":\"%s\"}",
        name, kind, theme);
    HttpResponse<String> res = send(authed(HttpRequest.newBuilder(uri("customers"))
        .header("Content-Type", "application/json")
        .POST(HttpRequest.BodyPublishers.ofString(body))));
    if (res.statusCode() != 201) {
      throw new IllegalStateException("seedCustomer failed: " + res.statusCode() + " " + res.body());
    }
    String sysId = JsonParser.parseString(res.body()).getAsJsonObject().get("sysId").getAsString();
    return Map.of("sysId", sysId, "name", name);
  }

  public int deleteCustomer(String sysId) {
    return send(authed(HttpRequest.newBuilder(uri("customers/" + sysId)).DELETE())).statusCode();
  }

  public int customerStatus(String sysId) {
    return send(authed(HttpRequest.newBuilder(uri("customers/" + sysId)).GET())).statusCode();
  }

  private String get(String path) {
    return send(authed(HttpRequest.newBuilder(uri(path)).GET())).body();
  }

  private String firstSysId(String jsonArray) {
    return JsonParser.parseString(jsonArray).getAsJsonArray().get(0)
        .getAsJsonObject().get("sysId").getAsString();
  }

  private HttpRequest.Builder authed(HttpRequest.Builder builder) {
    String header = cookies.entrySet().stream()
        .map(e -> e.getKey() + "=" + e.getValue())
        .collect(Collectors.joining("; "));
    return builder.header("Cookie", header);
  }

  private URI uri(String path) {
    return URI.create(Config.apiBaseUrl() + "/" + path);
  }

  private HttpResponse<String> send(HttpRequest.Builder builder) {
    try {
      return http.send(builder.build(), HttpResponse.BodyHandlers.ofString());
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }
}

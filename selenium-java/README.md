# Selenium — Web E2E Suite (Java + JUnit 5)

A gold-standard browser **end-to-end** suite for the MaelstromOps web app, in
**Java** with **Selenium 4** + **JUnit 5** and **Maven** — the enterprise/classic
stack, done idiomatically (Page Object Model, explicit waits, no driver wrangling).

## Layout

```
selenium-java/
├─ pom.xml
└─ src/test/java/com/maelstromops/qa/
   ├─ support/
   │  ├─ Config.java          # env-driven settings — no hard-coded URLs/creds
   │  ├─ DriverFactory.java   # headless Chrome; Selenium Manager resolves the driver
   │  └─ ApiClient.java       # API login + hybrid seed/clean (java.net.http)
   ├─ pages/                  # Page Object Model (BasePage + screens, no assertions)
   │  ├─ BasePage.java  LoginPage.java  HomePage.java
   │  ├─ CustomersPage.java  CustomerNewPage.java  CustomerProfilePage.java
   └─ tests/
      ├─ BaseTest.java        # driver lifecycle + cookie-injection auth
      ├─ AuthTest.java        # login negative paths (unauthenticated)
      ├─ SmokeTest.java       # authenticated shell + navigation
      └─ CustomerJourneyTest.java   # create-via-form, hybrid, grid, edit/delete
```

## Techniques demonstrated
- **Page Object Model done properly** — a `BasePage` with shared helpers; page
  objects expose intent and hold locators; **assertions live only in tests**.
- **Explicit waits everywhere** — `WebDriverWait` + `ExpectedConditions`; implicit
  wait is zero. No `Thread.sleep`.
- **No driver management** — Selenium Manager (built into Selenium 4) resolves the
  matching driver automatically.
- **API-injected authentication** — log in once via the API and inject the session
  cookies into the browser (the mature alternative to clicking the login form every
  test; fast, and never trips the API's auth rate-limit).
- **The hybrid pattern** — seed/clean data via the API, assert through the UI.
- **A real create-through-the-form journey** — fills the New Customer form,
  including the Syncfusion **dropdowns**, committed with **real keyboard events**
  (`Actions.sendKeys(ARROW_DOWN, ENTER)`) — which the widget honours (a notable
  contrast with synthetic-event tools).
- **On-screen edit + delete** — the profile's View⇄Edit toggle and Delete,
  accepting the native confirm **alert**; deletion verified via the API.
- **Resilient selectors** — `data-testid`, input ids, accessible button text.

## Running

```bash
BASE_URL=http://localhost:5173 \
MOPS_API_BASE_URL=http://localhost:5009/api/v1 \
CHROME_BINARY=/usr/bin/chromium \
mvn test
```

Requires a running MaelstromOps app + a Chrome/Chromium browser. Same-site cookie
note applies (see the Playwright README): drive the UI on the same host its API
uses. CI runs the suite when a `BASE_URL` target is configured.

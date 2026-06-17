# Selenium — Web E2E Suite (C# + NUnit)

A gold-standard browser **end-to-end** suite for the MaelstromOps web app, in **C#**
with **Selenium 4** + **NUnit** — the .NET counterpart to the Java Selenium suite.

## Layout
```
selenium-csharp/
├─ Support/   Config.cs · DriverFactory.cs · ApiClient.cs
├─ Pages/     Pages.cs        # Page Object Model (BasePage + screens, no asserts)
└─ Tests/     BaseTest.cs · AuthTests.cs · SmokeTests.cs · CustomerJourneyTests.cs
```

## Techniques demonstrated
- **Page Object Model** — `BasePage` + screens, locators centralised, assertions
  only in tests. **Explicit waits** (custom lambda conditions); implicit wait = 0.
- **No driver management** — Selenium Manager resolves the driver automatically.
- **API-injected authentication** — log in once via the API and inject the session
  cookies into the browser (shared one login per run; dodges the auth rate-limit).
- **Hybrid pattern** — seed/clean via the API, assert through the UI.
- **Full journey** — create-through-the-form (Syncfusion dropdowns via `Actions`
  real keys), grid "View" navigation, on-screen edit + delete (accepting the native
  alert). JS-click helper for controls below the fold.

## Running
```bash
BASE_URL=http://localhost:5173 \
MOPS_API_BASE_URL=http://localhost:5009/api/v1 \
CHROME_BINARY=/usr/bin/chromium \
dotnet test
```
Requires a running MaelstromOps app + a Chrome/Chromium browser. CI builds on every
push and runs the suite when a `BASE_URL` target is configured.

# Playwright — Web E2E Suite (C# + NUnit)

A gold-standard browser **end-to-end** suite for the MaelstromOps web app, in **C#**
with **Microsoft.Playwright** + **NUnit** — the .NET counterpart to the TypeScript
Playwright suite, idiomatic for the .NET ecosystem.

## Layout
```
playwright-csharp/
├─ Support/   Config.cs · ApiClient.cs        # env config + API (hybrid + auth cookies)
├─ Pages/     Pages.cs                          # Page Object Model (locators, no asserts)
└─ Tests/     BaseTest.cs · AuthTests.cs · SmokeTests.cs · CustomerJourneyTests.cs
```

## Techniques demonstrated
- **Page Object Model** — page objects expose intent and hold locators
  (`data-testid`, input ids, accessible roles); assertions stay in tests.
- **Web-first assertions** — `Expect(locator).ToBeVisibleAsync()` auto-waits.
- **API-injected authentication** — log in once via the API and inject the session
  cookies into the browser context (`Context.AddCookiesAsync`); shared across the
  run, so it never trips the auth rate-limit.
- **The hybrid pattern** — seed/clean via the API, assert through the UI.
- **A real create-through-the-form journey** — incl. the Syncfusion dropdowns,
  committed with real keyboard events (`Page.Keyboard.PressAsync`).
- **On-screen edit + delete** — View⇄Edit + Delete, handling the native dialog.

## Running
```bash
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install chromium   # one-time browser install
BASE_URL=http://localhost:5173 MOPS_API_BASE_URL=http://localhost:5009/api/v1 dotnet test
```
Requires a running MaelstromOps app. CI builds on every push and runs the suite
when a `BASE_URL` target is configured.

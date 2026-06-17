# Playwright — Web E2E Suite (TypeScript)

A gold-standard browser **end-to-end** suite for the MaelstromOps web app, written
in **TypeScript** with **Playwright Test**. Like the API suite, it's written to be
read: a technique-by-technique reference for maintainable UI automation.

## Layout

```
playwright-ts/
├─ src/
│  ├─ config.ts            # env-driven settings — no hard-coded URLs/creds
│  ├─ api.ts               # API helper for hybrid setup/teardown (seed via API)
│  ├─ fixtures.ts          # custom fixtures injecting the page objects
│  └─ pages/               # Page Object Model
│     ├─ LoginPage.ts
│     ├─ HomePage.ts
│     ├─ CustomersPage.ts
│     └─ CustomerNewPage.ts
├─ tests/
│  ├─ auth.setup.ts        # log in once → save authenticated storage state
│  ├─ auth.spec.ts         # login UI: negative paths (runs unauthenticated)
│  ├─ smoke.spec.ts        # authenticated shell + navigation (reuses session)
│  └─ customers.spec.ts    # full customer journey: create-via-form, hybrid, grid
├─ playwright.config.ts    # projects: setup → chromium; trace/video on failure
└─ package.json
```

## Techniques demonstrated
- **Page Object Model** — pages expose intent (`loginPage.login(...)`,
  `customersPage.expectLoaded()`); selectors live in one place.
- **Resilient selectors** — stable `data-testid` surfaces, input `id`s, and
  accessible roles (`getByRole`) over brittle CSS/XPath.
- **Authenticate once, reuse everywhere** — a `setup` project logs in through the
  real UI and saves storage state; every other test starts authenticated. Fast,
  and it avoids hammering the API's auth rate-limit.
- **Per-test state override** — the login specs opt out of the shared session to
  exercise the unauthenticated form.
- **Custom fixtures** — page objects injected pre-built, so specs stay declarative.
- **Web-first assertions** — `expect(locator).toBeVisible()` auto-waits; no manual
  sleeps.
- **A real create-through-the-form journey** — fills the actual New Customer form,
  including Syncfusion **dropdowns** (opened via their wrapper, committed with
  keyboard ArrowDown+Enter — the list-item click doesn't reliably commit), then
  asserts the app lands on the new record's profile.
- **The hybrid pattern** — seed/clean test data through the **API** (fast,
  reliable) and assert through the **UI**; data setup never drives the expensive
  UI path.
- **Grid interaction** — drives the data grid's "View" command to open a profile.
- **Guaranteed cleanup** — `try/finally` deletes seeded data even on failure.
- **Debuggability on failure** — trace on first retry, screenshot + video retained
  on failure.
- **Config-driven environments** — point `BASE_URL` at local dev or the live demo.

## Running

```bash
npm install
npx playwright install --with-deps chromium

npm test                 # headless run (setup → chromium)
npm run test:headed      # watch it drive a real browser
npm run report           # open the HTML report
```

Requires a running MaelstromOps web app. Defaults target `http://localhost:5173`
(UI) and `http://localhost:5009/api/v1` (API). Override via env:

```bash
BASE_URL=https://maelstromops.com \
MOPS_API_BASE_URL=https://api.maelstromops.com/api/v1 \
MOPS_TEST_EMAIL=you@example.com MOPS_TEST_PASSWORD=••••• npm test
```

> **Same-site cookie note:** the auth cookie only flows when the UI and API share
> a site. With a local UI that points its API at a LAN IP, drive the UI on that
> same host (e.g. `BASE_URL=http://192.168.x.x:5173`) so the cookie is sent.

## Still to layer in
- **Edit + delete through the UI** (the profile's view⇄edit toggle and delete
  affordance) to round out the on-screen CRUD.
- Cross-browser **projects** (firefox/webkit) and CI (GitHub Actions).

# Cypress — Web E2E Suite (TypeScript)

A gold-standard browser **end-to-end** suite for the MaelstromOps web app, in
TypeScript with **Cypress** — a deliberate counterpart to the Playwright suite
(same app, same journeys, different tool), demonstrating fluency across the two
dominant E2E frameworks and where their tradeoffs show.

## Layout

```
cypress-ts/
├─ cypress.config.ts             # config-driven baseUrl + env (API, creds)
├─ cypress/
│  ├─ support/
│  │  ├─ commands.ts             # cy.login (cy.session) + cy.seedCustomer/deleteCustomer
│  │  └─ e2e.ts                  # support entry + custom-command types
│  └─ e2e/
│     ├─ auth.cy.ts              # login UI: negative paths (unauthenticated)
│     ├─ smoke.cy.ts             # authenticated shell + navigation
│     └─ customers.cy.ts         # hybrid, grid, and on-screen edit/delete
└─ package.json
```

## Techniques demonstrated
- **`cy.session` auth caching** — log in once (through the API) and reuse the
  session across tests, with a `validate()` re-check. This is Cypress's analogue
  to Playwright's saved storage state.
- **Custom commands** — `cy.login`, `cy.seedCustomer`, `cy.deleteCustomer`
  (typed) keep specs declarative.
- **The hybrid pattern** — seed/clean data via the API (`cy.request`), assert
  through the UI.
- **Grid interaction** and **on-screen edit + delete** — drives the grid's "View"
  command and the profile's Edit/Save + Delete (Cypress auto-accepts the native
  confirm dialog).
- **Resilient selectors** — `data-testid`, input `id`s, accessible button text.
- **Retries** in run mode; **config-driven** environments via `BASE_URL` + env.

## A documented framework-difference finding
The New Customer form's required kind/theme fields are **Syncfusion dropdowns**
whose selection only commits on a **real keyboard event** (ArrowDown+Enter).
Cypress drives the DOM with **synthetic** events (`.click()`, `.trigger`), which
the widget doesn't honour — so *create-through-the-form* can't be driven reliably
in Cypress. That exact journey lives in the **Playwright** suite, whose real (CDP)
keyboard events commit the selection. A concrete example of when the tool matters.

## Running

```bash
npm install
BASE_URL=http://localhost:5173 \
MOPS_API_BASE_URL=http://localhost:5009/api/v1 \
npm test                 # headless (cypress run)
npm run open             # interactive runner
```

Requires a running MaelstromOps app. Same-site note applies (see the Playwright
README): drive the UI on the same host its API uses so the auth cookie flows. CI
(`.github/workflows/cypress-ts.yml`) typechecks on every push and runs the full
suite when a `BASE_URL` repo variable is configured.

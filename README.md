# QA Automation Portfolio

A reference collection of **gold-standard, technique-by-technique** test automation
suites — each framework built as a teaching-grade example of how to structure and
write maintainable, professional tests. The suites exercise a real, non-trivial
target: **MaelstromOps**, a multi-tenant Quality & Software-Engineering SaaS
(React/TypeScript + C#/.NET + PostgreSQL).

> Why a real app instead of a demo site? Testing genuine auth, multi-tenant
> isolation, RBAC, pagination/search, and full CRUD workflows demonstrates far more
> than a tutorial sandbox. Every suite is **config-driven** (point it at any
> instance via env vars), so the same suites serve as the product's CI tests.

## Suites

| Suite | Language | Framework | Layer | Status |
|-------|----------|-----------|-------|--------|
| [`pytest-api/`](./pytest-api) | Python | Pytest + httpx | API | ✅ complete |
| [`playwright-ts/`](./playwright-ts) | TypeScript | Playwright | Web E2E | ✅ complete |
| [`cypress-ts/`](./cypress-ts) | TypeScript | Cypress | Web E2E | ✅ complete |
| [`selenium-java/`](./selenium-java) | Java | Selenium + JUnit | Web E2E | ✅ complete |
| [`k6-performance/`](./k6-performance) | JavaScript | k6 | Load/Perf | ✅ complete |
| [`playwright-csharp/`](./playwright-csharp) | C# | Playwright | Web E2E | ✅ complete |
| `selenium-csharp/` | C# | Selenium + NUnit | Web E2E | planned |

Each suite has its own README documenting **exactly which techniques it
demonstrates** and how to run it.

## What every suite aims to show
- Clean structure (Page Object Model / API-client layering) and separation of concerns
- Config-driven environments — no hard-coded URLs or credentials
- Data-driven / parameterized tests; happy-path **and** negative, boundary, and edge cases
- Setup/teardown, fixtures, and test independence (no order dependence, self-cleaning)
- Response/schema validation and precise assertions with readable failure messages
- Tagging/markers, selective runs, and parallel execution
- Reporting and CI (GitHub Actions)

## Running
Each suite is self-contained — see its README. All target a running MaelstromOps
API/UI; defaults assume a local instance, overridable via environment variables.

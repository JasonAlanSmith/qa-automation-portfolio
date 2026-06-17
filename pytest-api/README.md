# Pytest — API Test Suite

A gold-standard **API** test suite for MaelstromOps, written in Python with
**Pytest** + **httpx**. It's built to be read as much as run: a deliberate,
technique-by-technique reference for how to structure and write maintainable API
tests.

## Layout

```
pytest-api/
├─ src/
│  ├─ config.py        # env-driven settings — no hard-coded URLs/creds
│  ├─ api_client.py    # the "Page Object Model" for an API: intent-revealing client
│  ├─ factories.py     # test-data builders (unique, valid-by-default payloads)
│  └─ schemas.py       # jsonschema contracts for response-shape validation
├─ tests/
│  ├─ test_authentication.py      # login / me / refresh-rotation / logout, negative auth
│  ├─ test_health_smoke.py        # list endpoints answer with the paged envelope
│  ├─ test_customer_crud.py       # full create→read→update→delete lifecycle
│  ├─ test_pagination_search.py   # paging contract, search filtering, boundaries
│  └─ test_tenant_isolation.py    # multi-tenant scoping invariant (+ gated cross-tenant)
├─ conftest.py         # fixtures: session auth, reference resolution, self-cleaning factory
├─ pytest.ini          # markers, strict config
└─ requirements.txt
```

## Techniques demonstrated
- **Config-driven environments** — everything via env vars / `.env`; point
  `MOPS_BASE_URL` at any instance (local or the cloud demo) with zero code changes.
- **Client abstraction (POM for APIs)** — tests express intent (`login`,
  `get("/customers")`); transport details live in one place.
- **Fixtures & deliberate scope** — session-scoped authenticated client (log in
  once) vs. function-scoped anonymous client for auth-flow isolation; reference
  FKs resolved once per run.
- **Test-data factories** — `factories.py` builds unique, valid-by-default
  payloads; tests state only what matters to them.
- **Self-cleaning CRUD via a factory fixture** — every resource a test creates is
  tracked and deleted in teardown, even on failure → no residue, full test
  independence (order never matters).
- **Parameterization** — one test body, many cases (invalid-credential matrix;
  every list endpoint; page-size boundaries).
- **Negative, boundary & error-path testing** — wrong password, unknown user,
  empty input, unauthenticated access, unknown ids, missing required fields.
- **Schema/contract validation** — `jsonschema` checks the *shape* of responses
  (`me`, customer, reference items, the paged envelope), catching silent contract
  drift, not just status codes.
- **Pagination & search** — page-size honoured, envelope metadata coherent,
  non-overlapping page coverage, server-side search incl. the empty-result case.
- **Multi-tenant isolation** — asserts every row a caller sees belongs to their
  tenant; an honest, config-gated cross-tenant test that *skips* rather than fakes
  when a second tenant isn't provided.
- **Markers** — `smoke`, `regression`, `auth`, `crud`, `negative`, `tenant` for
  selective runs and build gating.
- **Full session lifecycle** — login → `me` → **refresh (token rotation)** → logout.

## A documented finding: parallel runs vs. the auth rate-limit
The tests are designed to be parallel-safe — independent, uniquely-named data,
self-cleaning. They are **not** order- or worker-dependent. However, the target
API enforces a **per-IP auth rate-limit of 10 requests/minute** (a deliberate
brute-force control). Under `pytest -n auto`, many workers burst-login at once and
the API correctly returns **`429 Too Many Requests`**.

This is surfaced rather than hidden: it's a real interaction with the target's
security posture, not a suite defect. The suite therefore **runs serially by
default**. To exercise parallelism, point the suite at an instance whose auth
rate-limit is relaxed for testing (`MOPS_BASE_URL`), or run a non-auth subset.

## Running

```bash
python3 -m venv .venv
.venv/bin/pip install -r requirements.txt
cp .env.example .env            # adjust if not using local defaults

.venv/bin/pytest                       # run everything (serial)
.venv/bin/pytest -m smoke              # just the smoke gate
.venv/bin/pytest -m "crud or tenant"   # selective by marker
.venv/bin/pytest --html=reports/report.html --self-contained-html   # HTML report
```

Targets a running MaelstromOps API (defaults to `http://localhost:5009/api/v1`).
Set `MOPS_TEST_EMAIL_2` / `MOPS_TEST_PASSWORD_2` to a second-tenant account to
enable the cross-tenant isolation test (otherwise it skips).

## Still to layer in
- **CI** (GitHub Actions) — lint + collect on every push; full run when a target
  is configured. *(workflow included; see `.github/workflows/`.)*
- Opt-in **retries** (`pytest-rerunfailures`) for genuinely flaky network calls.

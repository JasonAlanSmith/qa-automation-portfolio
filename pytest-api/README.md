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
│  └─ schemas.py       # jsonschema contracts for response-shape validation
├─ tests/
│  ├─ test_authentication.py
│  └─ test_health_smoke.py
├─ conftest.py         # fixtures (session-scoped auth, function-scoped anon client)
├─ pytest.ini          # markers, strict config
└─ requirements.txt
```

## Techniques demonstrated
- **Config-driven environments** — everything via env vars / `.env`; point
  `MOPS_BASE_URL` at any instance (local or the cloud demo) with zero code changes.
- **Client abstraction (POM for APIs)** — tests express intent (`login`,
  `get("/products")`); transport details live in one place.
- **Fixtures & deliberate scope** — session-scoped authenticated client (log in
  once) vs. function-scoped anonymous client for auth-flow isolation.
- **Parameterization** — one test body, many cases (invalid-credential matrix;
  every list endpoint).
- **Negative & error-path testing** — wrong password, unknown user, empty input,
  unauthenticated access.
- **Schema/contract validation** — `jsonschema` checks the *shape* of responses,
  catching silent contract drift, not just status codes.
- **Markers** — `smoke`, `regression`, `auth`, `negative` (+ `crud`, `tenant` as
  the suite grows) for selective runs and build gating.
- **Full session lifecycle** — login → `me` → **refresh (token rotation)** → logout.

## Roadmap (being layered in)
- Full **CRUD lifecycle** with setup/teardown and self-cleanup (create → read →
  update → delete), proving test independence.
- **Test-data builders/factories** for readable, intention-revealing payloads.
- **Multi-tenant isolation** tests (a hallmark of the target app).
- **Pagination, search, and boundary** cases.
- **Parallel execution** (`pytest-xdist -n auto`), **HTML reporting**, opt-in
  **retries** for genuinely flaky network calls, and **CI** (GitHub Actions).

## Running

```bash
python3 -m venv .venv
.venv/bin/pip install -r requirements.txt
cp .env.example .env            # adjust if not using local defaults

.venv/bin/pytest                # run everything
.venv/bin/pytest -m smoke       # just the smoke gate
.venv/bin/pytest -n auto        # parallel
.venv/bin/pytest --html=reports/report.html --self-contained-html
```

Targets a running MaelstromOps API (defaults to `http://localhost:5009/api/v1`).

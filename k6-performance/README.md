# k6 — API Performance / Load Suite

A gold-standard **performance** suite for the MaelstromOps API, written for **k6**
(JavaScript). A different dimension from the functional suites: it asserts the API
holds its **SLOs under concurrency**, not just that endpoints work.

## Layout

```
k6-performance/
├─ config.js            # env-driven base URL + credentials
├─ helpers.js           # one-time API login → reusable auth cookie
└─ api-read-load.js     # scenarios + thresholds + checks (the load test)
```

## What it does
- **Two scenarios:** a single-user `smoke` (proves the path works under no load),
  then a `ramping-vus` `load` profile (ramp to 10 VUs, hold, ramp down).
- **Thresholds as pass/fail SLO gates** — the run *fails* if they're breached:
  - `http_req_failed: rate<0.01` (< 1% errors)
  - `http_req_duration: p(95)<800` (95% under 800 ms)
  - a custom `list_endpoint_duration` trend, also `p(95)<800`
  - `checks: rate>0.99`
- **Authenticate once** in `setup()` and reuse the session across all VUs — one hit
  on the rate-limited auth endpoint; the load itself targets read/list endpoints.
- **Grouped, tagged requests** with response **checks** (status + paged-envelope
  shape) so failures are attributable per endpoint.

Last local run (10 VUs): **checks 100%, errors 0%, p95 ≈ 60 ms** — comfortably
inside the SLOs.

## A k6-specific finding worth knowing
The access cookie (`maelstrom_auth`) is **HttpOnly**, and k6's `res.cookies` hides
HttpOnly *values* (length 0). Reading them back from the **cookie jar**
(`http.cookieJar().cookiesForURL(...)`) returns the real value — that's how
`helpers.js` builds the reusable auth header. (Sending the empty value gave a
silent 401 storm under load — a good reminder to verify auth before trusting load
numbers.)

## Running

```bash
MOPS_API_BASE_URL=http://localhost:5009/api/v1 \
MOPS_TEST_EMAIL=test@test.org MOPS_TEST_PASSWORD=test \
k6 run api-read-load.js

k6 run --out json=results/run.json api-read-load.js   # machine-readable output
```

Requires a running MaelstromOps API. CI validates the script on every push
(`k6 inspect`) and runs the load test when an `MOPS_API_BASE_URL` target is set.

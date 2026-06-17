/**
 * Environment-driven settings — never hard-code URLs or credentials in tests.
 *
 * `BASE_URL` is the MaelstromOps web app under test (the React UI). `API_BASE_URL`
 * is the same deployment's API, used only for fast test-data setup/teardown (the
 * hybrid pattern: seed via API, verify through the UI).
 *
 * Defaults target a local dev instance. NOTE: the UI and API must be same-site for
 * the auth cookie to flow — when the local UI points its API at a LAN IP, drive the
 * UI on that same host too (e.g. BASE_URL=http://192.168.x.x:5173).
 */
export const config = {
  baseUrl: process.env.BASE_URL ?? 'http://localhost:5173',
  apiBaseUrl: process.env.MOPS_API_BASE_URL ?? 'http://localhost:5009/api/v1',
  email: process.env.MOPS_TEST_EMAIL ?? 'test@test.org',
  password: process.env.MOPS_TEST_PASSWORD ?? 'test',

  /** Where the authenticated browser state is saved by the setup project. */
  storageState: 'playwright/.auth/user.json',
} as const;

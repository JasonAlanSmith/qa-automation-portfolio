import { defineConfig } from 'cypress';

/**
 * Config-driven: BASE_URL is the web app under test; the API base + credentials
 * ride along in `env` for the hybrid pattern (seed via API, assert via UI).
 * Defaults target a local dev instance.
 */
export default defineConfig({
  e2e: {
    baseUrl: process.env.BASE_URL ?? 'http://localhost:5173',
    specPattern: 'cypress/e2e/**/*.cy.ts',
    supportFile: 'cypress/support/e2e.ts',
    video: false,
    retries: { runMode: 2, openMode: 0 },
    env: {
      apiBaseUrl:
        process.env.MOPS_API_BASE_URL ?? 'http://localhost:5009/api/v1',
      email: process.env.MOPS_TEST_EMAIL ?? 'test@test.org',
      password: process.env.MOPS_TEST_PASSWORD ?? 'test',
    },
  },
});

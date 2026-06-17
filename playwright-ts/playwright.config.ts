import { defineConfig, devices } from '@playwright/test';

import { config } from './src/config';

/**
 * Playwright configuration.
 *
 * - A `setup` project logs in once and saves the authenticated storage state;
 *   every other test reuses it (no per-test login → fast, and it sidesteps the
 *   API's auth rate-limit instead of hammering it).
 * - `trace`/`screenshot`/`video` are captured on failure for debuggability.
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [['html', { open: 'never' }], ['list']],

  use: {
    baseURL: config.baseUrl,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },

  projects: [
    // 1) Authenticate once; persist the session to disk.
    { name: 'setup', testMatch: /auth\.setup\.ts/ },

    // 2) The real suite — runs authenticated by reusing the saved state, across
    //    all three engines. The saved cookies are browser-agnostic, so one login
    //    serves every project.
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'], storageState: config.storageState },
      dependencies: ['setup'],
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'], storageState: config.storageState },
      dependencies: ['setup'],
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'], storageState: config.storageState },
      dependencies: ['setup'],
    },
  ],
});

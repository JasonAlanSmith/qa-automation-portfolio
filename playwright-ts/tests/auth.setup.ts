import { expect, test as setup } from '@playwright/test';

import { config } from '../src/config';
import { LoginPage } from '../src/pages/LoginPage';

/**
 * Authentication setup: log in through the real UI exactly once, then persist the
 * authenticated browser state to disk. Every other project reuses it, so the rest
 * of the suite never logs in again — fast, and it doesn't pummel the API's auth
 * rate-limit the way per-test login would.
 */
setup('authenticate', async ({ page }) => {
  const login = new LoginPage(page);
  await login.goto();
  await login.login(config.email, config.password);

  // A successful login redirects to /home — wait for it before capturing state.
  await page.waitForURL('**/home');
  await expect(page.getByTestId('home')).toBeVisible();

  await page.context().storageState({ path: config.storageState });
});

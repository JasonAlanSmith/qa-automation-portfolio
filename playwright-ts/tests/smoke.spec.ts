import { expect, test } from '../src/fixtures';

/**
 * Authenticated smoke: these run with the shared, pre-authenticated storage state
 * (from auth.setup.ts) — no login here. They prove the session is reused and the
 * core authenticated surfaces render.
 */
test.describe('authenticated shell', () => {
  test('home loads for an authenticated user', async ({ homePage, page }) => {
    await homePage.goto();
    await homePage.expectLoaded();
    // We're authenticated — not bounced back to the login screen.
    await expect(page).not.toHaveURL(/\/login/);
  });

  test('navigates to the Customers browse grid', async ({ customersPage }) => {
    await customersPage.goto();
    await customersPage.expectLoaded();
  });
});

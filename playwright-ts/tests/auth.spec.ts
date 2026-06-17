import { config } from '../src/config';
import { expect, test } from '../src/fixtures';

/**
 * Login UI behaviour — these tests drive the login form itself, so they run
 * UNauthenticated (override the suite's shared storage state to a clean one).
 *
 * Demonstrated techniques: per-test storage-state override, negative paths,
 * resilient web-first assertions.
 */
test.use({ storageState: { cookies: [], origins: [] } });

test.describe('login', () => {
  test('rejects a wrong password and stays on the login page', async ({
    loginPage,
    page,
  }) => {
    await loginPage.goto();
    await loginPage.login(config.email, 'definitely-wrong');

    // The app does not navigate to /home on failure; the login surface remains.
    // (Finding: the UI currently shows no inline error on a rejected login — a
    // UX gap worth flagging. We assert the observable behaviour: no redirect.)
    await expect(loginPage.root).toBeVisible();
    await expect(page).not.toHaveURL(/\/home/);
  });

  test('rejects an unknown user', async ({ loginPage, page }) => {
    await loginPage.goto();
    await loginPage.login('nobody@nowhere.test', 'whatever-password');

    await expect(loginPage.root).toBeVisible();
    await expect(page).not.toHaveURL(/\/home/);
  });
});

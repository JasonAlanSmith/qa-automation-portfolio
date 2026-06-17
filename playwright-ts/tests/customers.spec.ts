import { createApiContext, deleteCustomer, seedCustomer } from '../src/api';
import { expect, test } from '../src/fixtures';

/**
 * Customer journey through the real UI.
 *
 * Demonstrated techniques:
 * - a full create-through-the-form user journey (incl. Syncfusion dropdowns)
 * - the HYBRID pattern: seed/clean data via the API, assert through the UI
 * - grid interaction (row → profile navigation)
 * - guaranteed cleanup via try/finally so tests leave no residue
 */
const uniqueName = (prefix = 'pw-customer'): string =>
  `${prefix}-${Date.now()}-${Math.floor(Math.random() * 10000)}`;

const PROFILE_URL = /\/customer\/browse\/[0-9a-f-]{36}\/profile/;

test.describe('customer journey', () => {
  test('create a customer through the form and land on its profile', async ({
    customerNewPage,
    page,
  }) => {
    const name = uniqueName();

    await customerNewPage.goto();
    await customerNewPage.createOrganization(name);

    // The app navigates to the new customer's profile ONLY on a successful create.
    await page.waitForURL(PROFILE_URL);
    await expect(page.getByTestId('customer.profile')).toBeVisible();

    // Clean up via API using the id from the resulting URL.
    const sysId = /browse\/([0-9a-f-]{36})\/profile/.exec(page.url())![1];
    const api = await createApiContext();
    await deleteCustomer(api, sysId);
    await api.dispose();
  });

  test('a customer seeded via the API renders in the UI (hybrid)', async ({
    page,
  }) => {
    const api = await createApiContext();
    const customer = await seedCustomer(api, uniqueName('pw-seeded'));

    try {
      await page.goto(`/customer/browse/${customer.sysId}/profile`);
      const profile = page.getByTestId('customer.profile');
      await expect(profile).toBeVisible();
      // The profile renders read-only, so the name appears as text (not an input).
      await expect(profile).toContainText(customer.name);
    } finally {
      await deleteCustomer(api, customer.sysId);
      await api.dispose();
    }
  });

  test('opening a customer from the grid shows its profile', async ({
    customersPage,
    page,
  }) => {
    await customersPage.goto();
    await customersPage.openFirstProfile();

    await page.waitForURL(PROFILE_URL);
    await expect(page.getByTestId('customer.profile')).toBeVisible();
  });
});

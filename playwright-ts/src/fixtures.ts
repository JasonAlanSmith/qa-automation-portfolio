import { test as base } from '@playwright/test';

import { CustomerNewPage } from './pages/CustomerNewPage';
import { CustomersPage } from './pages/CustomersPage';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';

/**
 * Custom test fixtures: each Page Object is injected pre-built, so specs read as
 * intent (`homePage.expectLoaded()`) rather than wiring. This is the Playwright
 * analogue of the API suite's client/factory fixtures.
 */
type Pages = {
  loginPage: LoginPage;
  homePage: HomePage;
  customersPage: CustomersPage;
  customerNewPage: CustomerNewPage;
};

export const test = base.extend<Pages>({
  loginPage: async ({ page }, use) => {
    await use(new LoginPage(page));
  },
  homePage: async ({ page }, use) => {
    await use(new HomePage(page));
  },
  customersPage: async ({ page }, use) => {
    await use(new CustomersPage(page));
  },
  customerNewPage: async ({ page }, use) => {
    await use(new CustomerNewPage(page));
  },
});

export { expect } from '@playwright/test';

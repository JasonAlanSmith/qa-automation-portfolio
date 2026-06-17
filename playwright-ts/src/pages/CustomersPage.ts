import { expect, type Locator, type Page } from '@playwright/test';

/** Page Object for the Customers browse grid. */
export class CustomersPage {
  readonly page: Page;
  readonly root: Locator;

  constructor(page: Page) {
    this.page = page;
    this.root = page.getByTestId('customer.browse');
  }

  async goto(): Promise<void> {
    await this.page.goto('/customer/browse');
  }

  async expectLoaded(): Promise<void> {
    await expect(this.root).toBeVisible();
  }
}

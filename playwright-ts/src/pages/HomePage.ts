import { expect, type Locator, type Page } from '@playwright/test';

/** Page Object for the authenticated landing page (the app shell's Home). */
export class HomePage {
  readonly page: Page;
  readonly root: Locator;

  constructor(page: Page) {
    this.page = page;
    this.root = page.getByTestId('home');
  }

  async goto(): Promise<void> {
    await this.page.goto('/home');
  }

  async expectLoaded(): Promise<void> {
    await expect(this.root).toBeVisible();
  }
}

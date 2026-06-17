import { expect, type Locator, type Page } from '@playwright/test';

/**
 * Page Object for the "New Customer" form.
 *
 * The kind/theme fields are Syncfusion dropdowns: clicking the field opens a
 * popup of options. We select the first available option rather than a specific
 * label, so the journey doesn't depend on which reference rows happen to be seeded.
 */
export class CustomerNewPage {
  readonly page: Page;
  readonly root: Locator;
  readonly name: Locator;
  readonly submit: Locator;

  constructor(page: Page) {
    this.page = page;
    this.root = page.getByTestId('customer.new');
    this.name = page.locator('#name');
    this.submit = page.getByRole('button', { name: 'Create Customer' });
  }

  async goto(): Promise<void> {
    await this.page.goto('/customer/new');
    await expect(this.root).toBeVisible();
  }

  private async pickFirstOption(fieldId: string): Promise<void> {
    // Open via the wrapper (the readonly input intercepts clicks), wait for the
    // popup, then select the first real option by keyboard. Clicking the popup
    // <li> doesn't reliably commit the Syncfusion selection — ArrowDown (off the
    // "-- Select --" placeholder) + Enter does.
    await this.page.locator(`span.e-ddl:has(input#${fieldId})`).click();
    await this.page.locator('.e-popup-open .e-list-item').first().waitFor();
    await this.page.keyboard.press('ArrowDown');
    await this.page.keyboard.press('Enter');
  }

  async createOrganization(name: string): Promise<void> {
    await this.name.fill(name);
    await this.pickFirstOption('kindSysId');
    await this.pickFirstOption('themeSysId');
    await this.submit.click();
  }
}

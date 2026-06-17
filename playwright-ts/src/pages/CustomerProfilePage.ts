import { expect, type Locator, type Page } from '@playwright/test';

/**
 * Page Object for a customer's profile, which has a read-only View that toggles
 * into Edit (Save/Cancel) and a Delete that confirms via a native dialog.
 */
export class CustomerProfilePage {
  readonly page: Page;
  readonly root: Locator;
  readonly editButton: Locator;
  readonly saveButton: Locator;
  readonly deleteButton: Locator;
  readonly nameInput: Locator;

  constructor(page: Page) {
    this.page = page;
    this.root = page.getByTestId('customer.profile');
    this.editButton = page.getByRole('button', { name: 'Edit' });
    this.saveButton = page.getByRole('button', { name: 'Save' });
    this.deleteButton = page.getByRole('button', { name: 'Delete' });
    this.nameInput = page.locator('#name');
  }

  async goto(sysId: string): Promise<void> {
    await this.page.goto(`/customer/browse/${sysId}/profile`);
    await expect(this.root).toBeVisible();
  }

  /** Enter edit mode, change the name, and save (drops back to read-only view). */
  async rename(newName: string): Promise<void> {
    await this.editButton.click();
    await this.nameInput.fill(newName);
    await this.saveButton.click();
  }

  /** Click Delete and accept the browser confirm dialog it raises. */
  async deleteCustomer(): Promise<void> {
    this.page.once('dialog', (dialog) => void dialog.accept());
    await this.deleteButton.click();
  }
}

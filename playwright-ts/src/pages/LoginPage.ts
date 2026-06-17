import { expect, type Locator, type Page } from '@playwright/test';

/**
 * Page Object for the login screen.
 *
 * Selectors prefer stable hooks over brittle CSS: the surface `data-testid`
 * scopes the page, inputs are addressed by their stable `id`, and the button by
 * its accessible role/name.
 */
export class LoginPage {
  readonly page: Page;
  readonly root: Locator;
  readonly email: Locator;
  readonly password: Locator;
  readonly submit: Locator;

  constructor(page: Page) {
    this.page = page;
    this.root = page.getByTestId('auth.login');
    this.email = page.locator('#email');
    this.password = page.locator('#password');
    this.submit = page.getByRole('button', { name: 'Log In' });
  }

  async goto(): Promise<void> {
    await this.page.goto('/login');
    await expect(this.root).toBeVisible();
  }

  async login(email: string, password: string): Promise<void> {
    await this.email.fill(email);
    await this.password.fill(password);
    await this.submit.click();
  }
}

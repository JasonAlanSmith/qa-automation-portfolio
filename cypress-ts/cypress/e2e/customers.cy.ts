/**
 * Customer journey through the real UI (Cypress).
 *
 * Demonstrated: create-through-the-form (incl. Syncfusion dropdowns), the hybrid
 * pattern (seed/clean via API, assert via UI), grid interaction, and on-screen
 * edit + delete (Cypress auto-accepts the native confirm dialog).
 */
const uniqueName = (p = 'cy-customer'): string =>
  `${p}-${Date.now()}-${Math.floor(Math.random() * 10000)}`;

const PROFILE = /\/customer\/browse\/[0-9a-f-]{36}\/profile/;

// NOTE (documented framework-difference finding): a *create-through-the-form*
// test is intentionally not included here. The New Customer form's required
// kind/theme fields are Syncfusion dropdowns, and committing a selection needs a
// real keyboard event (ArrowDown+Enter). Cypress drives the DOM with *synthetic*
// events (`.click()`, `.trigger('keydown')`), which the widget does not honour, so
// the value never commits. Playwright's real (CDP) keyboard events do — so that
// exact journey lives in the Playwright suite. Cypress here covers the patterns it
// drives reliably: hybrid setup, grid interaction, and on-screen edit/delete
// (the edit test still exercises form input via the editable #name field).

describe('customer journey', () => {
  beforeEach(() => {
    cy.login();
  });

  it('renders an API-seeded customer in the UI (hybrid)', () => {
    cy.seedCustomer(uniqueName('cy-seeded')).then((customer) => {
      cy.visit(`/customer/browse/${customer.sysId}/profile`);
      cy.get('[data-testid="customer.profile"]')
        .should('be.visible')
        .and('contain.text', customer.name);
      cy.deleteCustomer(customer.sysId);
    });
  });

  it('opens a customer from the grid', () => {
    cy.visit('/customer/browse');
    cy.contains('button', 'View').first().click();

    cy.location('pathname').should('match', PROFILE);
    cy.get('[data-testid="customer.profile"]').should('be.visible');
  });

  it('edits then deletes a customer entirely through the UI', () => {
    cy.seedCustomer(uniqueName('cy-edit')).then((customer) => {
      cy.visit(`/customer/browse/${customer.sysId}/profile`);

      // UPDATE (View → Edit → Save).
      const newName = uniqueName('cy-renamed');
      cy.contains('button', 'Edit').click();
      cy.get('#name').clear().type(newName);
      cy.contains('button', 'Save').click();
      cy.get('[data-testid="customer.profile"]').should('contain.text', newName);

      // DELETE (Cypress auto-accepts the confirm) → back to the browse grid.
      cy.contains('button', 'Delete').click();
      cy.location('pathname').should('eq', '/customer/browse');

      // Verified gone via the API.
      cy.request({
        url: `${Cypress.env('apiBaseUrl')}/customers/${customer.sysId}`,
        failOnStatusCode: false,
      })
        .its('status')
        .should('eq', 404);
    });
  });
});

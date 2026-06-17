/**
 * Authenticated smoke — uses the cached session from cy.login (no per-test login).
 */
describe('authenticated shell', () => {
  beforeEach(() => {
    cy.login();
  });

  it('home loads for an authenticated user', () => {
    cy.visit('/home');
    cy.get('[data-testid="home"]').should('be.visible');
    cy.location('pathname').should('not.include', '/login');
  });

  it('navigates to the Customers browse grid', () => {
    cy.visit('/customer/browse');
    cy.get('[data-testid="customer.browse"]').should('be.visible');
  });
});

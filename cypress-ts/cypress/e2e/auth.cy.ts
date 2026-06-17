/**
 * Login UI behaviour — runs unauthenticated (no cy.login).
 *
 * Demonstrated: negative paths, resilient selectors (data-testid + input ids +
 * accessible button text), retry-backed assertions.
 */
describe('login', () => {
  it('rejects a wrong password and stays on the login page', () => {
    cy.visit('/login');
    cy.get('[data-testid="auth.login"]').should('be.visible');

    cy.get('#email').type(Cypress.env('email'));
    cy.get('#password').type('definitely-wrong');
    cy.contains('button', 'Log In').click();

    // No navigation to /home on failure; the login surface remains.
    // (Finding: the UI shows no inline error on a rejected login.)
    cy.location('pathname').should('not.include', '/home');
    cy.get('[data-testid="auth.login"]').should('be.visible');
  });

  it('rejects an unknown user', () => {
    cy.visit('/login');
    cy.get('#email').type('nobody@nowhere.test');
    cy.get('#password').type('whatever-password');
    cy.contains('button', 'Log In').click();

    cy.location('pathname').should('not.include', '/home');
    cy.get('[data-testid="auth.login"]').should('be.visible');
  });
});

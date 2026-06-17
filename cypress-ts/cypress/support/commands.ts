/// <reference types="cypress" />

/**
 * Custom commands — the Cypress analogue of Playwright fixtures/page objects.
 *
 * `cy.login()` uses `cy.session` to authenticate ONCE and cache the session
 * across tests (Cypress's contrast to Playwright's saved storage state). The
 * login itself goes through the API for speed; the cached cookies then authorise
 * the UI. `cy.seedCustomer` / `cy.deleteCustomer` drive the hybrid pattern.
 */

const apiUrl = (path: string): string => `${Cypress.env('apiBaseUrl')}/${path}`;

Cypress.Commands.add('login', () => {
  cy.session(
    'test-user',
    () => {
      cy.request('POST', apiUrl('authentication/login'), {
        email: Cypress.env('email'),
        password: Cypress.env('password'),
      })
        .its('status')
        .should('eq', 200);
    },
    {
      // Validate the cached session is still good before reusing it.
      validate() {
        cy.request(apiUrl('authentication/me')).its('status').should('eq', 200);
      },
    },
  );
});

Cypress.Commands.add('seedCustomer', (name: string) => {
  return cy
    .request(apiUrl('references/customer-kinds'))
    .then((kinds) =>
      cy
        .request(apiUrl('references/customer-themes'))
        .then((themes) => ({ kind: kinds.body[0].sysId, theme: themes.body[0].sysId })),
    )
    .then(({ kind, theme }) =>
      cy
        .request('POST', apiUrl('customers'), {
          name,
          isIndividual: false,
          isActive: true,
          kindSysId: kind,
          themeSysId: theme,
        })
        .then((res) => {
          expect(res.status).to.eq(201);
          return { sysId: res.body.sysId as string, name };
        }),
    );
});

Cypress.Commands.add('deleteCustomer', (sysId: string) => {
  cy.request({
    method: 'DELETE',
    url: apiUrl(`customers/${sysId}`),
    failOnStatusCode: false,
  });
});

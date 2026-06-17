import './commands';

// Type declarations for the custom commands.
export interface SeededCustomer {
  sysId: string;
  name: string;
}

declare global {
  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace Cypress {
    interface Chainable {
      /** Authenticate once and cache the session across tests. */
      login(): Chainable<void>;
      /** Create a customer via the API; returns its id + name. */
      seedCustomer(name: string): Chainable<SeededCustomer>;
      /** Delete a customer via the API (no-op if already gone). */
      deleteCustomer(sysId: string): Chainable<void>;
    }
  }
}

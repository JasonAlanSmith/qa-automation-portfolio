import http from 'k6/http';
import { check } from 'k6';

import { config } from './config.js';

/**
 * Logs in once and returns a Cookie header for authenticated requests. Called from
 * setup() so the whole test authenticates a single time (one hit on the auth
 * endpoint, which is rate-limited) and every VU reuses the session.
 */
export function authenticate() {
  const res = http.post(
    `${config.apiBaseUrl}/authentication/login`,
    JSON.stringify({ email: config.email, password: config.password }),
    { headers: { 'Content-Type': 'application/json' }, tags: { name: 'login' } },
  );
  check(res, { 'login succeeded (200)': (r) => r.status === 200 });
  if (res.status !== 200) {
    throw new Error(`Login failed: ${res.status} ${res.body}`);
  }

  // The access cookie (maelstrom_auth) is HttpOnly, and k6's `res.cookies` hides
  // HttpOnly *values*. Read the real values from the cookie jar instead, then
  // return a Cookie header every VU can reuse (one login for the whole run).
  const jarCookies = http.cookieJar().cookiesForURL(config.apiBaseUrl);
  const pairs = [];
  for (const name in jarCookies) {
    pairs.push(`${name}=${jarCookies[name][0]}`);
  }
  return pairs.join('; ');
}
